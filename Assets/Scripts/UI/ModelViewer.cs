using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 3D モデルをマウス（指）のドラッグでくるくる回して見る画面。RawImage に付ける。
/// 画面の外に置いた専用カメラ（_viewCamera）でモデルを撮り、その映像をこの RawImage に映す。
/// ・回すのは _pivot。中のモデルは差し替えてよい（_pivot の子を入れ替えるだけ）
/// ・離したあとも少しだけ回り続ける（慣性）
/// ・ResetView で最初の向き（正面）にゆっくり戻る
/// ・アプリをプレハブにすると、シーンにあるカメラと軸への参照が外れる（プレハブはシーンの物を参照できない）。
///   そのときは、シーンの _stageName（BirdStage）の中から自分で探す
/// </summary>
[RequireComponent(typeof(RawImage))]
public class ModelViewer : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private const int MinTextureSize = 256;

    [Header("参照")]
    [SerializeField] private Camera _viewCamera;
    [SerializeField] private Transform _pivot;

    [Header("参照が空のときに探す、シーンの3Dの置き場の名前（中のカメラと Pivot を使う）")]
    [SerializeField] private string _stageName = "BirdStage";

    [Header("回り方")]
    [SerializeField] private float _rotateSpeed = 0.3f;
    [SerializeField] private float _inertiaDamping = 0.9f;
    [SerializeField] private float _resetDuration = 0.55f;

    private RawImage _rawImage;
    private RenderTexture _renderTexture;
    private Quaternion _baseRotation;
    private Vector2 _velocity;
    private bool _isDragging;

    /// <summary>
    /// モデルを撮っているカメラ（手紙がこちらを向いたかの判定に使う）
    /// </summary>
    public Camera ViewCamera => _viewCamera;

    /// <summary>
    /// 回している軸（中にモデルが入っている）
    /// </summary>
    public Transform Pivot => _pivot;

    /// <summary>
    /// 最初の向きを「正面」として覚えておく
    /// </summary>
    private void Awake()
    {
        _rawImage = GetComponent<RawImage>();
        FindStageIfMissing();
        _baseRotation = _pivot.rotation;
    }

    /// <summary>
    /// カメラか軸が空なら、シーンの3Dの置き場から探す（プレハブにしたときのため）
    /// </summary>
    private void FindStageIfMissing()
    {
        if (_viewCamera != null && _pivot != null)
        {
            return;
        }

        GameObject stage = GameObject.Find(_stageName);

        if (stage == null)
        {
            Debug.LogError($"[ModelViewer] シーンに {_stageName} が見つかりません");
            return;
        }

        if (_viewCamera == null)
        {
            _viewCamera = stage.GetComponentInChildren<Camera>(true);
        }

        if (_pivot == null)
        {
            _pivot = stage.transform.Find("Pivot");
        }
    }

    /// <summary>
    /// 画面が出たら、この大きさの映像を作ってカメラを動かす
    /// </summary>
    private void OnEnable()
    {
        Canvas.ForceUpdateCanvases();
        Rect rect = ((RectTransform)transform).rect;
        float scale = _rawImage.canvas != null ? _rawImage.canvas.scaleFactor : 1f;
        int width = Mathf.Max(MinTextureSize, Mathf.RoundToInt(rect.width * scale));
        int height = Mathf.Max(MinTextureSize, Mathf.RoundToInt(rect.height * scale));

        _renderTexture = new RenderTexture(width, height, 24);
        _viewCamera.targetTexture = _renderTexture;
        _viewCamera.aspect = (float)width / height;
        _rawImage.texture = _renderTexture;
        _viewCamera.enabled = true;
    }

    /// <summary>
    /// 画面が消えたら、カメラを止めて映像を捨てる（見えないあいだは撮らない）
    /// </summary>
    private void OnDisable()
    {
        _pivot.DOKill();
        _viewCamera.enabled = false;
        _viewCamera.targetTexture = null;
        _rawImage.texture = null;

        if (_renderTexture != null)
        {
            _renderTexture.Release();
            Destroy(_renderTexture);
            _renderTexture = null;
        }
    }

    /// <summary>
    /// 離したあとの慣性で回し、少しずつ止める
    /// </summary>
    private void Update()
    {
        if (_isDragging || _velocity.sqrMagnitude < 0.0001f)
        {
            return;
        }

        Rotate(_velocity);
        _velocity *= Mathf.Pow(_inertiaDamping, Time.deltaTime * 60f);
    }

    /// <summary>
    /// 押してドラッグを始めたとき（正面に戻る途中なら止める）
    /// </summary>
    public void OnBeginDrag(PointerEventData eventData)
    {
        _pivot.DOKill();
        _isDragging = true;
        _velocity = Vector2.zero;
    }

    /// <summary>
    /// ドラッグした分だけ回す
    /// </summary>
    public void OnDrag(PointerEventData eventData)
    {
        Rotate(eventData.delta);
        _velocity = eventData.delta;
    }

    /// <summary>
    /// 離したとき（止まってから離したなら、慣性はつけない）
    /// </summary>
    public void OnEndDrag(PointerEventData eventData)
    {
        _isDragging = false;

        if (eventData.delta.sqrMagnitude < 1f)
        {
            _velocity = Vector2.zero;
        }
    }

    /// <summary>
    /// 「正面」ボタン：最初の向きにゆっくり戻る
    /// </summary>
    public void ResetView()
    {
        _velocity = Vector2.zero;
        _pivot.DOKill();
        _pivot.DORotateQuaternion(_baseRotation, _resetDuration).SetEase(Ease.OutCubic).SetLink(gameObject);
    }

    /// <summary>
    /// すぐに最初の向きにする（アプリを開いたとき）
    /// </summary>
    public void SnapToBase()
    {
        _velocity = Vector2.zero;
        _pivot.DOKill();
        _pivot.rotation = _baseRotation;
    }

    /// <summary>
    /// 画面上の動き（ピクセル）に合わせて回す。横に動かすと左右に、縦に動かすと上下に回る
    /// </summary>
    private void Rotate(Vector2 delta)
    {
        Transform cameraTransform = _viewCamera.transform;
        _pivot.Rotate(cameraTransform.up, -delta.x * _rotateSpeed, Space.World);
        _pivot.Rotate(cameraTransform.right, delta.y * _rotateSpeed, Space.World);
    }
}

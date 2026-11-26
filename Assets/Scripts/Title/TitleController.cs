using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

/// <summary>
/// タイトル画面のスマホ（仕様書 v0.2 の4-1、デモの toTitle / startFromTitle / ドラッグ処理を移植）。
/// スマホが画面の下から上3/5だけのぞき、上にドラッグ（または ↑ / Enter / Space）すると中央へ上がって InGame へ進む。
/// InGame に入るときに新しいゲームになる（暗証番号の抽選・進行状況のリセット）。
/// スマホの画像（RectTransform。アンカーと中心は画面の真ん中）に付ける。ドラッグを受けるので Image の Raycast Target をオンにしておく
/// </summary>
[RequireComponent(typeof(RectTransform))]
public class TitleController : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    // 画面の高さの半分から「スマホの高さ × 0.1」を引いた分だけ下げると、スマホの上3/5が見える
    private const float PeekRate = 0.1f;

    // 上に行きすぎたとき・下に行きすぎたときの、指への追従の割合（ゴムのような手応え）
    private const float OverTopResistance = 0.25f;
    private const float UnderPeekResistance = 0.2f;

    // 開始の条件：のぞいている高さの30%以上上げる、または30以上を素早く払う
    private const float StartDistanceRate = 0.3f;
    private const float FlickMinDistance = 30f;
    private const float FlickMinSpeed = 450f;

    // 起動したときに、画面の下の外から出てくる位置の余白
    private const float EnterMargin = 40f;

    [Header("参照")]
    [SerializeField] private CanvasGroup _logo;

    [Header("演出の秒数（デモと同じ値）")]
    [SerializeField] private float _enterDuration = 1.1f;
    [SerializeField] private float _riseDuration = 0.64f;
    [SerializeField] private float _returnDuration = 0.42f;
    [SerializeField] private float _unlockDelay = 0.38f;

    private RectTransform _phone;
    private Canvas _canvas;
    private float _peekOffset;
    private bool _canOperate;
    private bool _isDragging;
    private bool _isStarting;
    private float _dragDistance;
    private float _dragSpeed;

    /// <summary>
    /// 自分のコンポーネントを取得する
    /// </summary>
    private void Awake()
    {
        _phone = GetComponent<RectTransform>();
        _canvas = GetComponentInParent<Canvas>();
    }

    /// <summary>
    /// のぞく位置を計算し、画面の下の外からのぞく位置までスマホを出す
    /// </summary>
    private void Start()
    {
        RectTransform area = (RectTransform)_phone.parent;
        float areaHeight = area.rect.height;
        float phoneHeight = _phone.rect.height;
        _peekOffset = Mathf.Max(0f, areaHeight / 2f - phoneHeight * PeekRate);

        SetRise(areaHeight / 2f + phoneHeight / 2f + EnterMargin);
        MoveTo(_peekOffset, _enterDuration, () => _canOperate = true);
    }

    /// <summary>
    /// キーボードでも始められるようにする（↑ / Enter / Space）
    /// </summary>
    private void Update()
    {
        if (!_canOperate || _isStarting || Keyboard.current == null)
        {
            return;
        }

        if (Keyboard.current.upArrowKey.wasPressedThisFrame
            || Keyboard.current.enterKey.wasPressedThisFrame
            || Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            StartGame();
        }
    }

    /// <summary>
    /// スマホをつかむ
    /// </summary>
    public void OnPointerDown(PointerEventData eventData)
    {
        if (!_canOperate || _isStarting)
        {
            return;
        }

        _phone.DOKill();
        _isDragging = true;
        _dragDistance = 0f;
        _dragSpeed = 0f;
    }

    /// <summary>
    /// 指に合わせてスマホを上下に動かす（のぞく位置より上・下では動きが重くなる）
    /// </summary>
    public void OnDrag(PointerEventData eventData)
    {
        if (!_isDragging)
        {
            return;
        }

        float deltaY = eventData.delta.y / _canvas.scaleFactor;
        _dragDistance += deltaY;
        _dragSpeed = deltaY / Mathf.Max(Time.unscaledDeltaTime, 0.0001f);

        SetRise(ApplyResistance(_peekOffset - _dragDistance));
    }

    /// <summary>
    /// 指を離したら、十分に上げていれば開始、足りなければのぞく位置へ戻す
    /// </summary>
    public void OnPointerUp(PointerEventData eventData)
    {
        if (!_isDragging)
        {
            return;
        }

        _isDragging = false;

        bool isFarEnough = _dragDistance > _peekOffset * StartDistanceRate;
        bool isFlicked = _dragDistance > FlickMinDistance && _dragSpeed > FlickMinSpeed;

        if (isFarEnough || isFlicked)
        {
            StartGame();
        }
        else
        {
            MoveTo(_peekOffset, _returnDuration, null);
        }
    }

    /// <summary>
    /// スマホを中央まで上げて、InGame へ進む（ボタンからも呼べる）
    /// </summary>
    public void StartGame()
    {
        if (!_canOperate || _isStarting)
        {
            return;
        }

        _isStarting = true;
        _isDragging = false;
        MoveTo(0f, _riseDuration, () =>
        {
            DOVirtual.DelayedCall(_unlockDelay, GoToInGame).SetLink(gameObject);
        });
    }

    /// <summary>
    /// 新しいゲームとして InGame へ切り替える（抽選は InGame に入ったところで行われる）
    /// </summary>
    private void GoToInGame()
    {
        GameProgress.RequestNewGame();
        SceneController.Load(GameScene.InGame);
    }

    /// <summary>
    /// スマホを指定の高さまで動かす（素早く動いてゆっくり止まる）
    /// </summary>
    private void MoveTo(float rise, float duration, TweenCallback onComplete)
    {
        _phone.DOKill();
        _phone.DOAnchorPosY(-rise, duration)
            .SetEase(Ease.OutQuart)
            .OnUpdate(UpdateLogo)
            .OnComplete(onComplete)
            .SetLink(gameObject);
    }

    /// <summary>
    /// スマホの高さを設定する（rise：中央からどれだけ下にあるか）
    /// </summary>
    private void SetRise(float rise)
    {
        Vector2 position = _phone.anchoredPosition;
        position.y = -rise;
        _phone.anchoredPosition = position;
        UpdateLogo();
    }

    /// <summary>
    /// のぞく位置より上・下に行きすぎたときに、動きを小さくする
    /// </summary>
    private float ApplyResistance(float rise)
    {
        if (rise < 0f)
        {
            return rise * OverTopResistance;
        }

        if (rise > _peekOffset)
        {
            return _peekOffset + (rise - _peekOffset) * UnderPeekResistance;
        }

        return rise;
    }

    /// <summary>
    /// スマホが上がるほどロゴを薄くする（のぞく位置で1、中央で0）
    /// </summary>
    private void UpdateLogo()
    {
        if (_logo == null)
        {
            return;
        }

        float rise = -_phone.anchoredPosition.y;
        _logo.alpha = _peekOffset > 0f ? Mathf.Clamp01(rise / _peekOffset) : 0f;
    }
}

using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// スマートフォンの画面を左右にスワイプして、
/// ロック画面をスライドさせるクラス。
/// </summary>
public class PhoneSwipe : MonoBehaviour
{
    [Header("画面設定")]
    [SerializeField] private RectTransform _frontScreen;

    [Header("スワイプ設定")]
    [SerializeField, Range(0.1f, 0.8f)]
    private float _unlockRate = 0.25f;

    [Header("アニメーション設定")]
    [SerializeField] private float _slideSpeed = 0.3f;
    [SerializeField] private float _returnSpeed = 0.25f;

    private Canvas _canvas;

    private Vector2 _startPosition;
    private Vector2 _originalPosition;

    private bool _isTouching = false;
    private bool _isAnimating = false;
    private bool _isUnlocked = false;

    private Coroutine _animationCoroutine;

    /// <summary>
    /// 必要なコンポーネントを取得する。
    /// </summary>
    private void Awake()
    {
        if (_frontScreen == null)
        {
            Debug.LogError("FrontScreenが設定されていません！");
            enabled = false;
            return;
        }

        _canvas = _frontScreen.GetComponentInParent<Canvas>();
    }

    /// <summary>
    /// 初期位置を保存する。
    /// </summary>
    private void Start()
    {
        _originalPosition = _frontScreen.anchoredPosition;
    }

    /// <summary>
    /// マウス入力を監視する。
    /// </summary>
    private void Update()
    {
        if (_isUnlocked || _isAnimating)
        {
            return;
        }

        if (Mouse.current == null)
        {
            return;
        }

        // マウスの左クリック開始
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            StartSwipe(Mouse.current.position.ReadValue());
        }

        // マウスをドラッグしている間
        if (_isTouching && Mouse.current.leftButton.isPressed)
        {
            MoveScreen(Mouse.current.position.ReadValue());
        }

        // マウスの左クリック終了
        if (_isTouching &&
            Mouse.current.leftButton.wasReleasedThisFrame)
        {
            EndSwipe(Mouse.current.position.ReadValue());
        }
    }

    /// <summary>
    /// マウスのドラッグ開始位置を記録する。
    /// </summary>
    private void StartSwipe(Vector2 position)
    {
        _startPosition = position;
        _isTouching = true;
    }

    /// <summary>
    /// マウスの移動量に合わせて画面を動かす。
    /// </summary>
    private void MoveScreen(Vector2 position)
    {
        float deltaX = position.x - _startPosition.x;

        float scale = _canvas != null ? _canvas.scaleFactor : 1f;

        float moveX = deltaX / scale;

        moveX = Mathf.Clamp(moveX, -_frontScreen.rect.width,_frontScreen.rect.width);

        _frontScreen.anchoredPosition = _originalPosition + new Vector2(moveX, 0);
    }

    /// <summary>
    /// ドラッグ終了時にロック解除判定を行う。
    /// </summary>
    private void EndSwipe(Vector2 position)
    {
        _isTouching = false;

        float deltaX = position.x - _startPosition.x;
        float deltaY = position.y - _startPosition.y;

        float scale = _canvas != null ? _canvas.scaleFactor : 1f;

        float unlockDistance = _frontScreen.rect.width * _unlockRate * scale;

        bool isHorizontal = Mathf.Abs(deltaX) > Mathf.Abs(deltaY);

        if (isHorizontal && Mathf.Abs(deltaX) >= unlockDistance)
        {
            float direction = Mathf.Sign(deltaX);

            _animationCoroutine = StartCoroutine(SlideOut(direction));
        }
        else
        {
            _animationCoroutine = StartCoroutine(ReturnScreen());
        }
    }

    /// <summary>
    /// 画面を左右どちらかの画面外までスライドさせる。
    /// </summary>
    private IEnumerator SlideOut(float direction)
    {
        _isAnimating = true;

        Vector2 targetPosition = _originalPosition + new Vector2( _frontScreen.rect.width * direction,0);

        Vector2 currentPosition =  _frontScreen.anchoredPosition;

        float elapsed = 0f;

        while (elapsed < _slideSpeed)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(elapsed / _slideSpeed);
            t = Mathf.SmoothStep(0f, 1f, t);

            _frontScreen.anchoredPosition = Vector2.Lerp(currentPosition, targetPosition, t);

            yield return null;
        }

        _frontScreen.anchoredPosition = targetPosition;

        _frontScreen.gameObject.SetActive(false);

        _isUnlocked = true;
        _isAnimating = false;

        _animationCoroutine = null;

        Debug.Log("スマホのロック解除成功！");
    }

    /// <summary>
    /// ドラッグ距離が足りない場合、元の位置に戻す。
    /// </summary>
    private IEnumerator ReturnScreen()
    {
        _isAnimating = true;

        Vector2 currentPosition = _frontScreen.anchoredPosition;

        float elapsed = 0f;

        while (elapsed < _returnSpeed)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(elapsed / _returnSpeed);
            t = Mathf.SmoothStep(0f, 1f, t);

            _frontScreen.anchoredPosition = Vector2.Lerp(currentPosition, _originalPosition,t);

            yield return null;
        }

        _frontScreen.anchoredPosition = _originalPosition;

        _isAnimating = false;

        _animationCoroutine = null;
    }
}

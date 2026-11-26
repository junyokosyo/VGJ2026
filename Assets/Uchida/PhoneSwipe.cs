using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/// <summary>
/// スマートフォンの画面を左右にスワイプして、
/// ロック画面をスライドさせるクラス。
/// LockScreen オブジェクト自身に付ける。表示されるたび（電池切れのやり直しのたび）に元の状態に戻る。
/// </summary>
public class PhoneSwipe : MonoBehaviour
{
    [Header("画面設定")]
    [SerializeField] private RectTransform _frontScreen;
    [SerializeField] private Image _fadeInPanel;

    [Header("スワイプ設定")]
    [SerializeField, Range(0.1f, 0.8f)]
    private float _unlockRate = 0.25f;

    [Header("アニメーション設定")]
    [SerializeField] private float _slideSpeed = 0.3f;
    [SerializeField] private float _returnSpeed = 0.25f;

    [Header("ロック解除したときに呼ぶ処理（PhoneNavigator.Unlock を設定する）")]
    [SerializeField] private UnityEvent _onUnlocked;

    [HideInInspector]
    public bool IsSwiping = false;

    private Canvas _canvas;

    private Color _color;

    private float _moveLength;
    private float _transformScale;

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
            enabled = false;
            return;
        }

        _canvas = _frontScreen.GetComponentInParent<Canvas>();

        // 初期位置を保存する（OnEnable は Start より先に呼ばれるので、ここで記録する）
        _originalPosition = _frontScreen.anchoredPosition;

        if (_fadeInPanel != null)
        {
            _color = _fadeInPanel.color;
        }
    }

    /// <summary>
    /// ロック画面が表示されるたび（電池切れのやり直しのたび）に、位置と状態を元に戻す。
    /// </summary>
    private void OnEnable()
    {
        if (_frontScreen == null)
        {
            return;
        }

        _frontScreen.anchoredPosition = _originalPosition;

        _isTouching = false;
        _isAnimating = false;
        _isUnlocked = false;
        _animationCoroutine = null;
        IsSwiping = false;

        SetFadeAlpha(1f);
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
        if (!IsInsideFrontScreen(position))
        {
            return;
        }
        else
        {
            _startPosition = position;
            _isTouching = true;
        }    
    }

    /// <summary>
    /// マウスの移動量に合わせて画面を動かす。
    /// </summary>
    private void MoveScreen(Vector2 position)
    {
        float deltaX = position.x - _startPosition.x;

        float scale = _canvas != null ? _canvas.scaleFactor : 1f;

        float moveX = deltaX / scale;

        moveX = Mathf.Clamp(moveX, -_frontScreen.rect.width, 0);

        _moveLength = Mathf.Max(0f,-deltaX);

        _frontScreen.anchoredPosition = _originalPosition + new Vector2(moveX, 0);

        UpdateFadeAlpha();
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

        _transformScale = scale;

        float unlockDistance = _frontScreen.rect.width * _unlockRate * scale;

        bool isHorizontal = Mathf.Abs(deltaX) > Mathf.Abs(deltaY);

        if (isHorizontal && -deltaX >= unlockDistance)
        {
            _animationCoroutine = StartCoroutine(SlideOut(-1f));
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

        _isUnlocked = true;
        _isAnimating = false;

        _animationCoroutine = null;

        IsSwiping = true;

        // 解除したことを知らせる（ここで PhoneNavigator がホーム画面に切り替える）
        _onUnlocked?.Invoke();

        _frontScreen.gameObject.SetActive(false);
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

        SetFadeAlpha(1f);
    }
    /// <summary>
    /// スワイプ距離に応じて透明度を変更する。
    /// </summary>
    private void UpdateFadeAlpha()
    {
        float scale = _canvas != null ? _canvas.scaleFactor : 1f;

        float unlockDistance =
            _frontScreen.rect.width * _unlockRate * scale;

        float progress = Mathf.Clamp01(_moveLength / unlockDistance);

        SetFadeAlpha(Mathf.Lerp(1f, 0f, progress));
    }

    /// <summary>
    /// フェード用パネルの透明度を設定する（パネルが未設定なら何もしない）。
    /// </summary>
    private void SetFadeAlpha(float alpha)
    {
        if (_fadeInPanel == null)
        {
            return;
        }

        _color.a = alpha;
        _fadeInPanel.color = _color;
    }
    /// <summary>
    /// マウスをクリックしたときにUIとかぶっているかを確認している。
    /// </summary>
    private bool IsInsideFrontScreen(Vector2 screenPosition)
    {
        Camera cam = null;
        if (_canvas != null && _canvas.renderMode != RenderMode.ScreenSpaceOverlay)
        {
            cam = _canvas.worldCamera;
        }
        return RectTransformUtility.RectangleContainsScreenPoint(_frontScreen, screenPosition, cam);
    }
}


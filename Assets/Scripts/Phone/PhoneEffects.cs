using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// スマホ全体の演出（デモ v0.11 と同じ動き）。Phone に付ける。
/// ・起動したとき：ステータスバーがふわっと出る
/// ・残り5%：上から「バッテリー残量が少なくなっています」の通知が出る（起動ごとに1回）
/// ・最後の1%（残り4秒）：画面がときどきチラつく
/// ・電源が落ちるとき：ブラウン管のように、横一本の線に縮んで白く光り、点になって消える
/// </summary>
public class PhoneEffects : MonoBehaviour
{
    [Header("参照")]
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private Timer _timer;
    [SerializeField] private Battery _battery;

    [Header("スマホの画面（Screen と、その CanvasGroup）")]
    [SerializeField] private RectTransform _screen;
    [SerializeField] private CanvasGroup _screenGroup;

    [Header("電源が落ちるときに白く光らせる画像（Screen いっぱい）")]
    [SerializeField] private Image _flash;

    [Header("ステータスバー")]
    [SerializeField] private CanvasGroup _statusBar;

    [Header("残りわずかの通知")]
    [SerializeField] private RectTransform _lowBatteryBanner;
    [SerializeField] private int _lowPercent = 5;
    [SerializeField] private float _bannerShowSeconds = 3.4f;
    [SerializeField] private float _bannerTop = 10f;

    [Header("秒数")]
    [SerializeField] private float _statusBarFadeDuration = 0.35f;
    [SerializeField] private float _crtDuration = 0.62f;
    [SerializeField] private float _flickerCycle = 1.3f;

    private Vector2 _screenBasePosition;
    private float _bannerHiddenY;
    private bool _hasWarned;
    private bool _isFlickering;
    private Sequence _bannerSequence;

    /// <summary>
    /// GameManager の知らせを受け取れるようにして、起動前の見た目にする
    /// </summary>
    private void Start()
    {
        _gameManager.OnBootStarted += HandleBootStarted;
        _gameManager.OnBooted += HandleBooted;
        _gameManager.OnPowerOffStarted += HandlePowerOffStarted;
        _gameManager.OnPoweredOff += HandlePoweredOff;

        _screenBasePosition = _screen.anchoredPosition;
        _bannerHiddenY = _lowBatteryBanner.rect.height + 20f;

        RestoreScreen();
        HideBanner();
        _statusBar.alpha = 0f;
    }

    /// <summary>
    /// 登録を解除する
    /// </summary>
    private void OnDestroy()
    {
        if (_gameManager == null)
        {
            return;
        }

        _gameManager.OnBootStarted -= HandleBootStarted;
        _gameManager.OnBooted -= HandleBooted;
        _gameManager.OnPowerOffStarted -= HandlePowerOffStarted;
        _gameManager.OnPoweredOff -= HandlePoweredOff;
    }

    /// <summary>
    /// 動いているあいだ、残りわずかの通知と、最後のチラつきを見る
    /// </summary>
    private void Update()
    {
        if (_gameManager.CurrentState != PhoneState.Running)
        {
            return;
        }

        if (!_hasWarned && _battery.Percent > 0 && _battery.Percent <= _lowPercent)
        {
            _hasWarned = true;
            ShowBanner();
        }

        bool isDying = _timer.RemainingTime <= _timer.Duration / 10f;

        if (isDying)
        {
            Flicker();
        }
        else if (_isFlickering)
        {
            StopFlicker();
        }
    }

    /// <summary>
    /// 起動が始まったら、前回の演出を元に戻す
    /// </summary>
    private void HandleBootStarted()
    {
        _hasWarned = false;
        RestoreScreen();
        HideBanner();
    }

    /// <summary>
    /// 起動したら、ステータスバーをふわっと出す
    /// </summary>
    private void HandleBooted()
    {
        _statusBar.DOKill();
        _statusBar.DOFade(1f, _statusBarFadeDuration).SetLink(gameObject);
    }

    /// <summary>
    /// 電源が落ちる演出（ブラウン管）
    /// </summary>
    private void HandlePowerOffStarted()
    {
        StopFlicker();
        HideBanner();

        float squeeze = _crtDuration * 0.45f;
        float shrink = _crtDuration * 0.3f;
        float vanish = _crtDuration * 0.25f;

        Sequence sequence = DOTween.Sequence().SetLink(gameObject);
        sequence.Append(_screen.DOScale(new Vector3(1f, 0.008f, 1f), squeeze).SetEase(Ease.InQuad));
        sequence.Join(_flash.DOFade(0.85f, squeeze));
        sequence.Append(_screen.DOScaleX(0.16f, shrink).SetEase(Ease.InQuad));
        sequence.Append(_screen.DOScale(Vector3.zero, vanish));
        sequence.Join(_screenGroup.DOFade(0f, vanish));
    }

    /// <summary>
    /// 電源が落ちきったら、画面を元の大きさに戻す（電池切れの画面を出すため）
    /// </summary>
    private void HandlePoweredOff()
    {
        RestoreScreen();
        _statusBar.DOKill();
        _statusBar.alpha = 0f;
    }

    /// <summary>
    /// 画面の大きさ・位置・明るさを元に戻す
    /// </summary>
    private void RestoreScreen()
    {
        _screen.DOKill();
        _screenGroup.DOKill();
        _flash.DOKill();

        _screen.localScale = Vector3.one;
        _screen.anchoredPosition = _screenBasePosition;
        _screenGroup.alpha = 1f;
        SetFlashAlpha(0f);
        _isFlickering = false;
    }

    /// <summary>
    /// 最後の1%のチラつき（_flickerCycle 秒ごとに、一瞬だけ暗くなって横にぶれる）
    /// </summary>
    private void Flicker()
    {
        _isFlickering = true;
        float phase = Time.time % _flickerCycle / _flickerCycle;
        float alpha = 1f;
        float offsetX = 0f;

        if (phase >= 0.46f && phase < 0.48f)
        {
            alpha = 0.78f;
            offsetX = 2f;
        }
        else if (phase >= 0.48f && phase < 0.50f)
        {
            offsetX = -1f;
        }
        else if (phase >= 0.50f && phase < 0.52f)
        {
            alpha = 0.9f;
        }

        _screenGroup.alpha = alpha;
        _screen.anchoredPosition = _screenBasePosition + new Vector2(offsetX, 0f);
    }

    /// <summary>
    /// チラつきを止める
    /// </summary>
    private void StopFlicker()
    {
        _isFlickering = false;
        _screenGroup.alpha = 1f;
        _screen.anchoredPosition = _screenBasePosition;
    }

    /// <summary>
    /// 残りわずかの通知を、上からすべり下ろして、少ししたら戻す
    /// </summary>
    private void ShowBanner()
    {
        _bannerSequence?.Kill();
        _bannerSequence = DOTween.Sequence().SetLink(gameObject);
        _bannerSequence.Append(_lowBatteryBanner.DOAnchorPosY(-_bannerTop, 0.4f).SetEase(Ease.OutQuart));
        _bannerSequence.AppendInterval(_bannerShowSeconds);
        _bannerSequence.Append(_lowBatteryBanner.DOAnchorPosY(_bannerHiddenY, 0.4f).SetEase(Ease.InQuad));
    }

    /// <summary>
    /// 通知を画面の上の外に隠す
    /// </summary>
    private void HideBanner()
    {
        _bannerSequence?.Kill();
        Vector2 position = _lowBatteryBanner.anchoredPosition;
        position.y = _bannerHiddenY;
        _lowBatteryBanner.anchoredPosition = position;
    }

    /// <summary>
    /// 白く光らせる画像の濃さを変える
    /// </summary>
    private void SetFlashAlpha(float alpha)
    {
        Color color = _flash.color;
        color.a = alpha;
        _flash.color = color;
    }
}

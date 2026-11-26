using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// スマホの画面（電源オフ・ロック・ホーム・各アプリ）の切り替えを管理する。
/// 画面の切り替えはすべてここを通す
/// </summary>
public class PhoneNavigator : MonoBehaviour
{
    [Header("参照")]
    [SerializeField] private GameManager _gameManager;

    [Header("画面")]
    [SerializeField] private GameObject _powerOffScreen;
    [SerializeField] private GameObject _lockScreen;
    [SerializeField] private GameObject _homeScreen;

    [Header("アプリ（AppBase を継承したもの）")]
    [SerializeField] private List<AppBase> _apps = new();

    private AppBase _currentApp;

    /// <summary>
    /// 今開いているアプリ（ホームやロック画面のときは null）
    /// </summary>
    public AppBase CurrentApp => _currentApp;

    /// <summary>
    /// GameManager のイベントを受け取れるようにして、電源オフの画面から始める
    /// </summary>
    private void Start()
    {
        _gameManager.OnBooted += ShowLockScreen;
        _gameManager.OnPoweredOff += ShowPowerOffScreen;

        foreach (AppBase app in _apps)
        {
            app.gameObject.SetActive(false);
        }

        ShowPowerOffScreen();
    }

    /// <summary>
    /// 登録を解除する
    /// </summary>
    private void OnDestroy()
    {
        if (_gameManager != null)
        {
            _gameManager.OnBooted -= ShowLockScreen;
            _gameManager.OnPoweredOff -= ShowPowerOffScreen;
        }
    }

    /// <summary>
    /// ロック画面からホームへ進む。ロック画面のボタンから呼ぶ
    /// </summary>
    public void Unlock()
    {
        if (!CanOperate())
        {
            return;
        }

        ShowOnly(_homeScreen);
    }

    /// <summary>
    /// アプリを開く。アプリアイコンから呼ばれる
    /// </summary>
    public void OpenApp(AppId appId)
    {
        if (!CanOperate())
        {
            return;
        }

        AppBase app = FindApp(appId);

        if (app == null)
        {
            Debug.LogWarning($"{appId} のアプリが PhoneNavigator に登録されていません");
            return;
        }

        CloseCurrentApp();
        ShowOnly(null);
        _currentApp = app;
        _currentApp.Open();
    }

    /// <summary>
    /// 開いているアプリを閉じてホームへ戻る。各アプリの戻るボタンから呼ぶ
    /// </summary>
    public void ReturnHome()
    {
        if (!CanOperate())
        {
            return;
        }

        CloseCurrentApp();
        ShowOnly(_homeScreen);
    }

    /// <summary>
    /// 起動が終わったらロック画面を出す。
    /// スワイプでロック画面をずらしたときに下からホーム画面が見えるよう、ホーム画面も後ろに表示しておく
    /// （ヒエラルキーで HomeScreen が LockScreen より上にあること。ロック画面がクリックを受け止めるので、アイコンは押せない）
    /// </summary>
    private void ShowLockScreen()
    {
        CloseCurrentApp();
        ShowOnly(_lockScreen);
        _homeScreen.SetActive(true);
    }

    /// <summary>
    /// 電源が落ちたら開いていたアプリを閉じ、電源ボタンの画面を出す
    /// </summary>
    private void ShowPowerOffScreen()
    {
        CloseCurrentApp();
        ShowOnly(_powerOffScreen);
    }

    /// <summary>
    /// 開いているアプリがあれば閉じる
    /// </summary>
    private void CloseCurrentApp()
    {
        if (_currentApp == null)
        {
            return;
        }

        _currentApp.Close();
        _currentApp = null;
    }

    /// <summary>
    /// 電源オフ・ロック・ホームのうち、指定した画面だけを表示する（null ならすべて隠す）
    /// </summary>
    private void ShowOnly(GameObject screen)
    {
        _powerOffScreen.SetActive(screen == _powerOffScreen);
        _lockScreen.SetActive(screen == _lockScreen);
        _homeScreen.SetActive(screen == _homeScreen);
    }

    /// <summary>
    /// 種類に合うアプリを探す
    /// </summary>
    private AppBase FindApp(AppId appId)
    {
        foreach (AppBase app in _apps)
        {
            if (app.AppId == appId)
            {
                return app;
            }
        }

        return null;
    }

    /// <summary>
    /// 操作を受け付けてよいか（起動中のみ。電源が落ちる演出中などは受け付けない）
    /// </summary>
    private bool CanOperate()
    {
        return _gameManager.CurrentState == PhoneState.Running;
    }
}

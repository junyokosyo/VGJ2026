using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ホーム画面のアプリアイコン。押すと指定したアプリを開く
/// </summary>
[RequireComponent(typeof(Button))]
public class AppIcon : MonoBehaviour
{
    [SerializeField] private PhoneNavigator _navigator;
    [SerializeField] private AppId _appId;

    private Button _button;

    /// <summary>
    /// 自分のボタンを取得する
    /// </summary>
    private void Awake()
    {
        _button = GetComponent<Button>();
    }

    /// <summary>
    /// ボタンが押されたときの処理を登録する
    /// </summary>
    private void Start()
    {
        _button.onClick.AddListener(OpenApp);
    }

    /// <summary>
    /// 登録を解除する
    /// </summary>
    private void OnDestroy()
    {
        _button.onClick.RemoveListener(OpenApp);
    }

    /// <summary>
    /// 指定したアプリを開く
    /// </summary>
    private void OpenApp()
    {
        _navigator.OpenApp(_appId);
    }
}

using UnityEngine;

/// <summary>
/// スマホのアプリの共通部分。各アプリはこれを継承して作る。
/// アプリの画面（パネル）のルートのオブジェクトに付ける
/// </summary>
public abstract class AppBase : MonoBehaviour
{
    [Header("このアプリの種類")]
    [SerializeField] private AppId _appId;

    /// <summary>
    /// このアプリの種類
    /// </summary>
    public AppId AppId => _appId;

    /// <summary>
    /// アプリを開く。PhoneNavigator だけが呼ぶ
    /// </summary>
    public void Open()
    {
        gameObject.SetActive(true);
        OnOpened();
    }

    /// <summary>
    /// アプリを閉じる。PhoneNavigator だけが呼ぶ（電源オフのときも呼ばれる）
    /// </summary>
    public void Close()
    {
        OnClosed();
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 開いたときに呼ばれる。進行状況（GameProgress）に合わせて表示を更新するのに使う
    /// </summary>
    protected virtual void OnOpened()
    {
        PasswordManager.Instance.ChangeType(_appId);
    }

    /// <summary>
    /// 閉じたときに呼ばれる。入力途中のパスワードを消すなど、画面を初期状態に戻すのに使う
    /// </summary>
    protected virtual void OnClosed()
    {
    }
}

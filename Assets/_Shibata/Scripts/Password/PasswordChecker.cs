using System;

public class PasswordChecker
{
    public PasswordChecker(GalleryApp galleryApp)
    {
        _galleryApp = galleryApp;
    }

    /// <summary>
    /// 自分の回答をリセット時
    /// </summary>
    public static event Action OnReset;

    private GalleryApp _galleryApp;

    /// <summary>
    /// 自分の回答が正しいか
    /// </summary>
    /// <param name="text">自分の回答</param>
    public void CheckAnswer(string text)
    {
        switch (PasswordManager.Instance.PuzzleType)
        {
            case PuzzleType.Lock:
                if (text == PasswordManager.Instance.Correct)
                {
                    UnityEngine.Debug.Log("Clear");
                    PasswordManager.Instance.ChangeType(PuzzleType.Gallery);
                    PasswordManager.Instance.InstancePassword();
                    PasswordManager.Instance.SetCorrect(PasswordManager.Instance.GalleryPassword.Correct);
                    OnReset?.Invoke();
                    _galleryApp.SetImages();
                }
                else OnReset?.Invoke();
                break;
            case PuzzleType.Gallery:
                if (PasswordManager.Instance.GalleryPassword.CheckAnswer(text))
                {
                    UnityEngine.Debug.Log("クリア");
                }
                else
                {
                    OnReset?.Invoke();
                }
                break;
        }
    }
}

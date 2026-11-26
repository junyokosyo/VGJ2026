using System;

public class PasswordChecker
{
    /// <summary>
    /// 自分の回答をリセット時
    /// </summary>
    public static event Action OnReset;

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
                    PasswordManager.Instance.ChangeType(PuzzleType.Picture);
                    PasswordManager.Instance.SetCorrect(PasswordManager.Instance.PicturrePassword.Correct);
                    PasswordManager.Instance.InstancePassword();
                    OnReset?.Invoke();
                }
                else OnReset?.Invoke();
                break;
            case PuzzleType.Picture:
                if (PasswordManager.Instance.PicturrePassword.CheckAnswer(text))
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

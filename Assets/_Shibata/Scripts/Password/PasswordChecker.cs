using System;

public class PasswordChecker
{
    public static event Action OnReset;

    public void CheckAnswer(string text)
    {
        switch (PasswordManager.Instance.PuzzleType)
        {
            case PuzzleType.Lock:
                if (text == PasswordManager.Instance.Correct)
                {
                    PasswordManager.Instance.ChangeType(PuzzleType.Picture);
                    PasswordManager.Instance.SetCorrect(PasswordManager.Instance.PicturrePassword.Correct);
                    OnReset?.Invoke();
                }
                else OnReset?.Invoke();
                break;
            case PuzzleType.Picture:
                if (PasswordManager.Instance.PicturrePassword.CheckAnswer(PasswordManager.Instance.Correct))
                {
                    UnityEngine.Debug.Log("クリア");
                }
                else OnReset?.Invoke();
                break;
        }
    }
}

using System;

public class PasswordChecker
{
    public static event Action OnReset;

    public void CheckAnswer(string text)
    {
        switch (PasswordManager.Instance.PuzzleType)
        {
            case PuzzleType.Lock:
                if (text == PasswordManager.Instance.Answer)
                {
                    PasswordManager.Instance.ChangeType(PuzzleType.Picture);
                    OnReset?.Invoke();
                }
                else OnReset?.Invoke();
                break;
            case PuzzleType.Picture:
                UnityEngine.Debug.Log("クリア");
                break;
        }
    }
}

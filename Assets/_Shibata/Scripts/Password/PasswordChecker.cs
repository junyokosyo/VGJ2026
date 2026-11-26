using System;
using System.Diagnostics;

public class PasswordChecker
{
    public static event Action OnReset;

    private string Answer()
    {
        if (PasswordManager.Instance.PuzzleType == PuzzleType.Lock)
        {
            return PasswordManager.Instance.LockAnswer;
        }
        else return PasswordManager.Instance.PictureAnswer;
    }

    public void CheckAnswer(string text)
    {
        if (text == Answer())
        {
            switch(PasswordManager.Instance.PuzzleType)
            {
                case PuzzleType.Lock:
                    PasswordManager.Instance.ChangeType(PuzzleType.Picture);
                    break;
                case PuzzleType.Picture:
                    UnityEngine.Debug.Log("クリア");
                    break;
            }
        }
        else
        {
            OnReset?.Invoke();
        }
    }
}

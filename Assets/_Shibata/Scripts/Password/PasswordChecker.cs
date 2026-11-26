using System;
using System.Diagnostics;
using UnityEngine;

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
                    UnityEngine.Debug.Log("Clear");
                    PasswordManager.Instance.ChangeType(PuzzleType.Picture);
                    PasswordManager.Instance.SetCorrect(PasswordManager.Instance.PicturrePassword.Correct);
                    PasswordManager.Instance.InstancePassword();
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

using System;
using System.Diagnostics;
using TMPro;
using UnityEngine.UI;

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
            //正解時の処理
        }
        else
        {
            UnityEngine.Debug.Log(text);
            OnReset?.Invoke();
        }
    }
}

using System.Collections.Generic;
using UnityEngine;
using Randam = UnityEngine.Random;

/// パスワードを管理する
[CreateAssetMenu(fileName = "AnswerDatabase", menuName = "Scriptable Objects/AnswerDatabase")]
public class AnswerDatabase : ScriptableObject
{
    public LockPassword LockPasswordAnswers;
    public List<ButtonData> PicturePasswords;
}

/// ロック画面のパスワード
[System.Serializable]
public class LockPassword
{
    private List<int> _birthMonth = new();
    private List<int> _birthDay = new();

    public int SelectBirthMonth => _selectBirthMonth;
    public int SelectBirthDay => _selectBirthDay;
    public string Birth => _birth;

    private int _selectBirthMonth;
    private int _selectBirthDay;
    private string _birth;

    public void SetBirthDay()
    {
        _birthDay.Clear();
        _birthMonth.Clear();

        for (int i = 1; i <= 12; i++)
        {
            _birthMonth.Add(i);
        }

        for (int i = 1; i <= 28; i++)
        {
            _birthDay.Add(i);
        }

        _selectBirthDay = _birthDay[Randam.Range(0, _birthDay.Count)];
        _selectBirthMonth = _birthMonth[Randam.Range(0, _birthMonth.Count)];

        string answer = _selectBirthMonth.ToString("D2") + _selectBirthDay.ToString("D2");
        PasswordManager.Instance.SetCorrect(answer);

        _birth = _selectBirthMonth + "月" + _selectBirthDay + "日";
    }
}

//ボタンのデータ
[System.Serializable]
public class ButtonData
{
    [Header("数字の画像")]
    public Sprite ImageSprite;
    [Header("画像の数字"),Range(0,9)]
    public int ID;
}
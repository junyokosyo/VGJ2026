using System.Collections.Generic;
using UnityEngine;
using Image = UnityEngine.UI.Image;
using Randam = UnityEngine.Random;

/// パスワードを管理する
[CreateAssetMenu(fileName = "AnswerDatabase", menuName = "Scriptable Objects/AnswerDatabase")]
public class AnswerDatabase : ScriptableObject
{
    public LockPassword RockPasswordAnswers;
}

/// ロック画面のパスワード
[System.Serializable]
public class LockPassword
{
    [Header("誕生月")]
    public List<int> BirthMonth = new();
    [Header("誕生日")]
    public List<int> BirthDay = new();

    public int SelectBirthMonth => _selectBirthMonth;
    public int SelectBirthDay => _selectBirthDay;
    private int _selectBirthMonth;
    private int _selectBirthDay;

    public void SetBirthDay()
    {
        BirthDay.Clear();
        BirthMonth.Clear();

        for (int i = 1; i <= 12; i++)
        {
            BirthMonth.Add(i);
        }

        for (int i = 1; i <= 28; i++)
        {
            BirthDay.Add(i);
        }

        _selectBirthDay = BirthDay[Randam.Range(0, BirthDay.Count)];
        _selectBirthMonth = BirthMonth[Randam.Range(0, BirthMonth.Count)];
    }
}

//写真のパスワード
[System.Serializable]
public class PicturePassword
{
    [Header("数字の画像")]
    public Sprite ImageSprite;
    [Header("画像の数字")]
    public int ID;
}
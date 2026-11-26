using System.Collections.Generic;
using UnityEngine;
using Randam = UnityEngine.Random;

/// パスワードを管理する
[CreateAssetMenu(fileName = "AnswerDatabase", menuName = "Scriptable Objects/AnswerDatabase")]
public class AnswerDatabase : ScriptableObject
{
    public LockPassword RockPasswordAnswers;
    public List<PicturePassword> PicturePasswords; 
}

/// ロック画面のパスワード
[System.Serializable]
public class LockPassword
{
    private List<int> _birthMonth = new();
    private List<int> _birthDay = new();

    public int SelectBirthMonth => _selectBirthMonth;
    public int SelectBirthDay => _selectBirthDay;
    private int _selectBirthMonth;
    private int _selectBirthDay;

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
    }
}

//ボタンのデータ
[System.Serializable]
public class PicturePassword
{
    [Header("数字の画像")]
    public Sprite ImageSprite;
    [Header("画像の数字"),Range(1,9)]
    public int ID;
}
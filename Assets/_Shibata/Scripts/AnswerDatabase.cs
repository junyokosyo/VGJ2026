using System.Collections.Generic;
using UnityEngine;
using Randam = UnityEngine.Random;

/// パスワードを管理する
[CreateAssetMenu(fileName = "AnswerDatabase", menuName = "Scriptable Objects/AnswerDatabase")]
public class AnswerDatabase : ScriptableObject
{
    /// <summary>
    /// ロック時のパスワード
    /// </summary>
    public LockPassword LockPasswordAnswers;
    /// <summary>
    /// 入力アイコンたち
    /// </summary>
    public List<ButtonData> PicturePasswords;
}

/// ロック画面のパスワード
[System.Serializable]
public class LockPassword
{
    /// <summary>
    /// 誕生月1～12まで
    /// </summary>
    private List<int> _birthMonth = new();
    /// <summary>
    /// 誕生日1～28まで
    /// </summary>
    private List<int> _birthDay = new();

    /// <summary>
    /// ランダムで選ばれた誕生月と日
    /// </summary>
    public string Birth => _birth;

    /// <summary>
    /// ランダムで選ばれた月
    /// </summary>
    private int _selectBirthMonth;
    /// <summary>
    /// ランダムで選ばれた日
    /// </summary>
    private int _selectBirthDay;
    private string _birth;

    public void SetBirthDay()
    {
        //リストを空にする
        _birthDay.Clear();
        _birthMonth.Clear();

        //誕生日と月の追加
        for (int i = 1; i <= 12; i++)
        {
            _birthMonth.Add(i);
        }

        for (int i = 1; i <= 28; i++)
        {
            _birthDay.Add(i);
        }

        //誕生日と月の抽選
        _selectBirthDay = _birthDay[Randam.Range(0, _birthDay.Count)];
        _selectBirthMonth = _birthMonth[Randam.Range(0, _birthMonth.Count)];

        //正解の答えを設定
        string birthCorrect = _selectBirthMonth.ToString("D2") + _selectBirthDay.ToString("D2");
        PasswordManager.Instance.SetCorrect(birthCorrect);

        //表示する月と日
        _birth = _selectBirthMonth + "月" + _selectBirthDay + "日";
    }
}

//ボタンのデータ
[System.Serializable]
public class ButtonData
{
    [Header("数字の画像")]
    public Sprite ImageSprite;
    [Header("画像の数字"), Range(0, 9)]
    public int ID;
}
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 謎に使うデータ（数字ボタンの画像・月ごとの写真・メモの文章）。
/// 答えそのものは持たない（答えは新しいゲームのたびに PasswordLottery が抽選し、GameProgress に保存する）
/// </summary>
[CreateAssetMenu(fileName = "AnswerDatabase", menuName = "Scriptable Objects/AnswerDatabase")]
public class AnswerDatabase : ScriptableObject
{
    /// <summary>
    /// ロック時のパスワード（柴田さんの誕生日のロック。今のゲームでは使っていないが、残しておく）
    /// </summary>
    [Header("ロック画面のパスワード（今は使っていない）")]
    public LockPassword LockPasswordAnswers;

    /// <summary>
    /// パスコード入力の数字ボタンの画像
    /// </summary>
    public List<ButtonData> PicturePasswords;

    /// <summary>
    /// Gallery に出す月ごとの写真（上から順に、抽選された枚数ぶん使う。足りなければ最後の1枚をくり返す）
    /// </summary>
    [Header("各月の写真（9〜12月）")]
    public List<ConfirmedSprite> confirmedSprites;

    /// <summary>
    /// Memo の4枚のメモの文章（9〜12月に1枚ずつ）
    /// </summary>
    [Header("メモアプリの文章（9〜12月）")]
    public List<Message> Messages;
}

/// <summary>
/// メモ1枚の文章
/// </summary>
[System.Serializable]
public class Message
{
    [Header("何月のメモか"), Range(9, 12)]
    public int Month;

    [Header("文章"), TextArea]
    public string Story;
}

/// <summary>
/// ある月の写真たち
/// </summary>
[System.Serializable]
public class ConfirmedSprite
{
    [Header("月"), Range(1, 12)]
    public int Month;
    public List<Sprite> Sprite;
}

/// <summary>
/// 数字ボタン1つ分の画像
/// </summary>
[System.Serializable]
public class ButtonData
{
    [Header("数字の画像")]
    public Sprite ImageSprite;

    [Header("画像の数字"), Range(0, 9)]
    public int ID;
}

/// ロック画面のパスワード（柴田さんの元のコード。今のゲームでは使っていないが、残しておく）
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
        _selectBirthDay = _birthDay[Random.Range(0, _birthDay.Count)];
        _selectBirthMonth = _birthMonth[Random.Range(0, _birthMonth.Count)];

        //正解の答えを設定
        string birthCorrect = _selectBirthMonth.ToString("D2") + _selectBirthDay.ToString("D2");
        PasswordManager.Instance.SetCorrect(birthCorrect);

        //表示する月と日
        _birth = _selectBirthMonth + "月" + _selectBirthDay + "日";
    }
}

using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Email アプリの文章（仕様書 v0.2 の5-1）。初期値はデモの文章。
/// 文章を直すときは、この Scriptable Object（Assets/Data/EmailData）をインスペクターで書き換える
/// </summary>
[CreateAssetMenu(fileName = "EmailData", menuName = "Scriptable Objects/EmailData")]
public class EmailData : ScriptableObject
{
    [Header("受信トレイ（本文は開けない）")]
    public List<InboxMail> InboxMails = new()
    {
        new InboxMail { From = "ひな", Subject = "とりさん、まだ？" },
        new InboxMail { From = "ひな", Subject = "こんどはいつあえる？" },
        new InboxMail { From = "パパ", Subject = "もうすこしだけ" },
        new InboxMail { From = "パパ", Subject = "ごめんね" },
    };

    [Header("下書き（本文を読める）")]
    public List<DraftMail> DraftMails = new()
    {
        new DraftMail
        {
            Subject = "件名なし",
            IsCut = true,
            Body = "もう時間がない。\n\nあの日のことは、もう忘れてほしい。\n\n写真も、手紙も、全部処分する。\n\nもう二度と、僕を探さなくていい。\n\nこれ以上、あの子を巻き込むわけにはいかない。\n\n僕がいなくなれば、全部終わる。\n\n誰にも話さないでほしい。\n\nさよな、",
        },
        new DraftMail
        {
            Subject = "件名なし",
            IsCut = false,
            Body = "9月、11月、12月、10月。\n\nメモを開くには、あの子との思い出の数を、この順番で確かめること。",
        },
    };

    [Header("受信メールを押したときに出す文")]
    public string CannotOpenText = "このメールの本文は表示できません";
}

/// <summary>
/// 受信メール1通
/// </summary>
[System.Serializable]
public class InboxMail
{
    public string From;
    public string Subject;
}

/// <summary>
/// 下書き1通
/// </summary>
[System.Serializable]
public class DraftMail
{
    public string Subject;

    [TextArea(5, 20)]
    public string Body;

    [Header("文が途中で切れているか（入力中の縦棒を出す）")]
    public bool IsCut;
}

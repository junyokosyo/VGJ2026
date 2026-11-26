using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Messaging アプリの文章（仕様書 v0.2 の5-5）。初期値はデモの文章。
/// 文章を直すときは、この Scriptable Object（Assets/Data/MessagingData）をインスペクターで書き換える
/// </summary>
[CreateAssetMenu(fileName = "MessagingData", menuName = "Scriptable Objects/MessagingData")]
public class MessagingData : ScriptableObject
{
    [Header("相手の名前")]
    public string Contact = "妻";

    [Header("隠された履歴の入口")]
    public string VaultTitle = "非表示のトーク履歴";
    public string VaultButton = "パスコードで表示";
    public string PastTitle = "隠された履歴";

    [Header("隠された履歴（弟と妻の過去のやり取り）")]
    public List<ChatLine> History = new()
    {
        new ChatLine { IsMine = true, Text = "ひなには、まだ話さないでおきたい。" },
        new ChatLine { IsMine = false, Text = "いつかは話さなきゃいけないよ。" },
        new ChatLine { IsMine = true, Text = "分かってる。でも、あの子に僕のことを心配しながら過ごしてほしくないんだ。" },
        new ChatLine { IsMine = false, Text = "ひなは、あなたに会いたがってるよ。" },
        new ChatLine { IsMine = true, Text = "だからこそ、待たせたくないんだ。" },
        new ChatLine { IsMine = false, Text = "あの鳥はどうするの？" },
        new ChatLine { IsMine = true, Text = "ちゃんと渡してあげたい。僕が渡せなくても。" },
    };

    [Header("最初から届いている妻のメッセージ")]
    public List<string> Today = new() { "スマートフォン、見つかったんだね。" };

    [Header("最初からある選択肢と、その返事（選ぶと Normal End）")]
    public string Choice = "何も分からなかった。";
    public List<string> ChoiceReplies = new() { "……そう。" };

    [Header("隠された履歴を見たあとに届く、ロッカーの話")]
    public List<string> Locker = new()
    {
        "そういえば、あの人の部屋にロッカーがあるんだけど……。",
        "何か大切なものを入れていたみたいなの。",
        "でも、パスワードが分からなくて開けられないの。",
    };

    [Header("番号を送ったあとの返事")]
    public List<string> OpenedReplies = new() { "開いた…。" };
    public List<string> FailedReplies = new() { "んー、これじゃ開かないみたいね。", "まあいいわ。" };

    [Header("ボタンの文字")]
    public string SendButton = "番号を送る";
    public string ToOpenButton = "ロッカーの中を見る";
    public string ToEndButton = "続ける";
}

/// <summary>
/// 会話の1行
/// </summary>
[System.Serializable]
public class ChatLine
{
    [Header("自分（弟）の発言なら ON")]
    public bool IsMine;

    [TextArea]
    public string Text;
}

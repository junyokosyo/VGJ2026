using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// エンディングの文章（仕様書 v0.2 の6章・デモ v0.27）。プランナーがインスペクターで直せるように、文章だけをまとめる。
/// 最初の値はデモ v0.27 の文章（Tools → NoSubject → Result を v0.27 に合わせる で、今のアセットも v0.27 の文章に書き換わる）
/// </summary>
[CreateAssetMenu(fileName = "EndingData", menuName = "NoSubject/EndingData")]
public class EndingData : ScriptableObject
{
    /// <summary>
    /// エンディング1つ分の文章
    /// </summary>
    [Serializable]
    public class Ending
    {
        public EndingType Type;
        public string Label;

        [TextArea(2, 8)]
        public string Lines;

        [Header("TRUE END だけ：手紙の本文（空なら手紙を出さない）。「ヒナ」「パパ」は手紙の絵の側にあるので、本文には入れない")]
        [TextArea(6, 20)]
        public string Letter;

        [Header("TRUE END だけ：結びの言葉の下の一文")]
        [TextArea(1, 3)]
        public string After;
    }

    [Header("エンディングの文章")]
    public List<Ending> Endings = new()
    {
        new Ending
        {
            Type = EndingType.True,
            Label = "TRUE END",
            Lines = TrueLines,
            Letter = TrueLetter,
            After = TrueAfter,
        },
        new Ending
        {
            Type = EndingType.Memo,
            Label = "MEMO END",
            Lines = MemoLines,
        },
        new Ending
        {
            Type = EndingType.Normal,
            Label = "NORMAL END",
            Lines = NormalLines,
        },
    };

    [Header("ロッカーが開いた画面（TRUE END）")]
    public string LockerPlace = "弟の部屋";
    public string LockerTitle = "ロッカーが開いた";
    public string LockerInside = "中に入っていたもの";
    public string LockerInsideSub = "娘に渡すためのもの";
    public string OpenLetter = "手紙をひらく";

    [Header("共通の文字")]
    public string Title = "件名なし。";
    public string RebootFormat = "再起動 {0}回";
    public string ClosePhone = "スマートフォンを閉じる";
    public string RecordFormat = "あなたの記録　再起動 {0}回";
    public string Again = "はじめから遊ぶ";

    // ───────── デモ v0.27 の文章（ツールからも使う） ─────────

    public const string TrueLines = "消そうとしたのは、思い出ではなく、\nひなが立ち止まる理由だった。";
    public const string TrueAfter = "鳥は、妻の手からひなのもとへ届けられた。";
    public const string TrueLetter =
        "とりさん、やっとできたよ。\n待っててくれて、ありがとう。\n\n" +
        "パパの写真も手紙も、ぜんぶ片づけました。\nひながパパを探して、立ち止まらないように。\n\n" +
        "でも、このとりさんだけは残したかった。\nパパの「だいすき」を、ぜんぶ詰めておいたからね。\n\n" +
        "パパのことは、忘れなくていい。\nでも、もう待っていなくていいんだよ。\n\n" +
        "ひなが笑うと、パパはいつだって幸せでした。";
    public const string MemoLines =
        "弟は、残された時間が少ないと知っていた。\nひなに心配をかけないよう、病気を隠していた。\n" +
        "写真にもメモにも、ひなとの思い出があふれていた。\nそして、あの鳥をひなに渡したがっていた。\n\n" +
        "けれど、ロッカーは開かなかった。\n鳥は、まだひなのもとへ届いていない。";
    public const string NormalLines = "「もう時間がない。」\nその言葉の意味は、最後まで分からなかった。";

    /// <summary>
    /// 種類に合うエンディングの文章を探す（なければ null）
    /// </summary>
    public Ending Find(EndingType type)
    {
        foreach (Ending ending in Endings)
        {
            if (ending.Type == type)
            {
                return ending;
            }
        }

        return null;
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// エンディングの文章（仕様書 v0.2 の6章）。プランナーがインスペクターで直せるように、文章だけをまとめる。
/// 最初の値はデモ（v0.11）の文章。MEMO END と NORMAL END は仮の文章
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

        [TextArea(2, 6)]
        public string Lines;

        [Header("TRUE END だけ：手紙の本文（空なら手紙を出さない）")]
        [TextArea(6, 20)]
        public string Letter;
    }

    [Header("エンディングの文章")]
    public List<Ending> Endings = new()
    {
        new Ending
        {
            Type = EndingType.True,
            Label = "TRUE END",
            Lines = "忘れてほしかったのではない。\n待ち続けてほしくなかった。\n\n鳥は妻の手から、娘のもとへ届けられた。",
            Letter = "ひなへ\n\nこれを渡すころには、パパはもう一緒に遊べないかもしれない。\n\nでも、この鳥はずっとひなのそばにいるよ。\n\nパパのことを忘れなくていい。\n\nだけど、パパを待たなくてもいいんだよ。\n\nたくさん遊んで、たくさん笑ってね。",
        },
        new Ending
        {
            Type = EndingType.Memo,
            Label = "MEMO END",
            Lines = "弟の想いは分かった。\nけれど、鳥は娘に届かなかった。",
        },
        new Ending
        {
            Type = EndingType.Normal,
            Label = "NORMAL END",
            Lines = "「もう時間がない。」\nその言葉の意味は、最後まで分からなかった。",
        },
    };

    [Header("共通の文字")]
    public string Title = "件名なし。";
    public string RebootFormat = "再起動 {0}回";

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

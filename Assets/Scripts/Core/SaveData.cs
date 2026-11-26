using System;
using System.Collections.Generic;

/// <summary>
/// ページを開き直しても残す、1プレイ分の状態（デモの S にあたる）。
/// GameProgress が JSON にして PlayerPrefs に保存する。直接さわらず、GameProgress を通して読み書きする
/// </summary>
[Serializable]
public class SaveData
{
    /// <summary>
    /// 保存形式の番号。形を変えたら上げる（古い保存データは読み込まずに新しいゲームにする）
    /// </summary>
    public const int CurrentVersion = 1;

    /// <summary>
    /// メモの最初の並び（シャッフル済み）。id：a=9月 b=10月 c=11月 d=12月
    /// </summary>
    public static readonly string[] DefaultMemoOrder = { "c", "a", "d", "b" };

    // JsonUtility で保存するため public フィールドにしている（データ専用のクラス）

    public int Version = CurrentVersion;

    /// <summary>その回の暗証番号の元</summary>
    public PuzzleCodes Codes = new();

    /// <summary>達成したこと（ProgressFlag を文字列にして持つ。enum の順番を変えても壊れないように）</summary>
    public List<string> Flags = new();

    /// <summary>再起動した回数（「もう一度起動する」で1増える。クリア記録に使う）</summary>
    public int RebootCount;

    /// <summary>メモの並び順（id の並び）</summary>
    public string[] MemoOrder = (string[])DefaultMemoOrder.Clone();

    /// <summary>Email で開いていたタブ（"inbox" か "drafts"）</summary>
    public string EmailTab = "inbox";

    /// <summary>妻に送った番号（まだなら空）</summary>
    public string SentNumber = "";

    /// <summary>たどり着いたエンディング（EndingType を文字列で）</summary>
    public string Ending = nameof(EndingType.None);

    /// <summary>記録画面で名前を記録したエンディング（同じエンディングで二重に記録しないため）</summary>
    public string RecordedEnding = "";
}

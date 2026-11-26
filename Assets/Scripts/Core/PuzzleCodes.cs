using System;

/// <summary>
/// 1プレイ分の暗証番号の元になる数字（仕様書 v0.2 の3-3）。
/// 抽選は柴田さんの担当。GameProgress.OnReset を受け取って抽選し、GameProgress.SetCodes で入れる。
/// ここは「入れておく場所」と「パスコードの組み立て方」だけを持つ
/// </summary>
[Serializable]
public class PuzzleCodes
{
    /// <summary>
    /// 月の数（9月・10月・11月・12月）
    /// </summary>
    public const int MonthCount = 4;

    /// <summary>
    /// 写真の枚数の範囲
    /// </summary>
    public const int MinPhotoCount = 3;
    public const int MaxPhotoCount = 9;

    /// <summary>
    /// 下書き2に書かれた月の順番（9月→11月→12月→10月）を、9・10・11・12月の並び（0〜3）で表したもの
    /// </summary>
    private static readonly int[] DraftOrder = { 0, 2, 3, 1 };

    // JsonUtility で保存するため public フィールドにしている（データ専用のクラス）

    /// <summary>
    /// 各月の写真の枚数（9月・10月・11月・12月の順、それぞれ3〜9）
    /// </summary>
    public int[] PhotoCounts = new int[MonthCount];

    /// <summary>
    /// 各メモのタイトルの数字（9月・10月・11月・12月のメモの順＝時系列順、それぞれ0〜9）
    /// </summary>
    public int[] MemoNumbers = new int[MonthCount];

    /// <summary>
    /// 鳥がくわえた手紙の裏の番号（"0000"〜"9999"）
    /// </summary>
    public string LetterNumber = "";

    /// <summary>
    /// Memo のパスコード（下書き2の順に写真の枚数を並べた4桁）
    /// </summary>
    public string MemoPassword
    {
        get
        {
            string password = "";

            foreach (int monthIndex in DraftOrder)
            {
                password += PhotoCounts[monthIndex].ToString();
            }

            return password;
        }
    }

    /// <summary>
    /// 隠された履歴のパスコード（メモを時系列に並べたときのタイトルの数字4桁）
    /// </summary>
    public string HistoryPassword
    {
        get
        {
            string password = "";

            foreach (int number in MemoNumbers)
            {
                password += number.ToString();
            }

            return password;
        }
    }

    /// <summary>
    /// ロッカーの番号（妻に送る番号。手紙の裏の番号と同じ）
    /// </summary>
    public string LockerPassword => LetterNumber;

    /// <summary>
    /// 仕様どおりの値が入っているか（抽選前や、保存データが壊れているときは false）
    /// </summary>
    public bool IsValid
    {
        get
        {
            if (PhotoCounts == null || PhotoCounts.Length != MonthCount || MemoNumbers == null || MemoNumbers.Length != MonthCount)
            {
                return false;
            }

            foreach (int count in PhotoCounts)
            {
                if (count < MinPhotoCount || count > MaxPhotoCount)
                {
                    return false;
                }
            }

            foreach (int number in MemoNumbers)
            {
                if (number < 0 || number > 9)
                {
                    return false;
                }
            }

            if (LetterNumber == null || LetterNumber.Length != 4 || !int.TryParse(LetterNumber, out _))
            {
                return false;
            }

            // 3つのパスコードは必ず別々（仕様書 3-3）
            return MemoPassword != HistoryPassword && HistoryPassword != LockerPassword && MemoPassword != LockerPassword;
        }
    }
}

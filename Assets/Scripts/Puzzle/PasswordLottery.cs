using UnityEngine;

/// <summary>
/// 暗証番号の元を抽選する（仕様書 v0.2 の3-3。柴田さんの MemoPassword の抽選を広げたもの）。
/// 新しいゲームのときに GameProgress から呼ばれる
/// </summary>
public static class PasswordLottery
{
    /// <summary>
    /// 写真の枚数・メモの数字・手紙の番号を抽選する。3つのパスコードが同じになったら引き直す
    /// </summary>
    public static PuzzleCodes Roll()
    {
        PuzzleCodes codes = new PuzzleCodes();

        do
        {
            for (int i = 0; i < PuzzleCodes.MonthCount; i++)
            {
                codes.PhotoCounts[i] = Random.Range(PuzzleCodes.MinPhotoCount, PuzzleCodes.MaxPhotoCount + 1);
                codes.MemoNumbers[i] = Random.Range(0, 10);
            }

            codes.LetterNumber = Random.Range(0, 10000).ToString("D4");
        }
        while (!codes.IsValid);

#if UNITY_EDITOR
        Debug.Log($"[PasswordLottery] Memo:{codes.MemoPassword} 履歴:{codes.HistoryPassword} ロッカー:{codes.LockerPassword}");
#endif

        return codes;
    }
}

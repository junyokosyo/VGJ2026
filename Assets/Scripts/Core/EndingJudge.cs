/// <summary>
/// エンディングを決める（仕様書 v0.2 の6章）。
/// どのエンディングも Messaging で妻に送る返事で決まる
/// </summary>
public static class EndingJudge
{
    /// <summary>
    /// 妻に送った番号からエンディングを決める（手紙の裏の番号と同じなら True、違えば Memo）
    /// </summary>
    public static EndingType JudgeSentNumber(string sentNumber)
    {
        return sentNumber == GameProgress.Codes.LockerPassword ? EndingType.True : EndingType.Memo;
    }

    /// <summary>
    /// 最初からある選択肢「何も分からなかった。」をまだ選べるか（隠された履歴を開くと消える）
    /// </summary>
    public static bool CanChooseNormalEnd()
    {
        return !GameProgress.HasFlag(ProgressFlag.HiddenHistoryUnlocked) && GameProgress.Ending == EndingType.None;
    }
}

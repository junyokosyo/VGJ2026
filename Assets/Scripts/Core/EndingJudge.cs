/// <summary>
/// 進行状況からエンディングを決める
/// </summary>
public static class EndingJudge
{
    /// <summary>
    /// 今の進行状況でゲームを終えたときのエンディングを返す。
    /// ※「履歴と鳥の数字は見つけたがロッカーは開けていない」場合は仕様が未定のため、いったん Memo End にしている
    /// </summary>
    public static EndingType Judge()
    {
        bool hasHistory = GameProgress.HasFlag(ProgressFlag.HiddenHistoryUnlocked);
        bool hasBirdNumber = GameProgress.HasFlag(ProgressFlag.BirdNumberFound);
        bool hasOpenedLocker = GameProgress.HasFlag(ProgressFlag.LockerOpened);

        if (hasHistory && hasBirdNumber && hasOpenedLocker)
        {
            return EndingType.True;
        }

        if (hasHistory)
        {
            return EndingType.Memo;
        }

        if (hasBirdNumber)
        {
            return EndingType.Bird;
        }

        return EndingType.Normal;
    }
}

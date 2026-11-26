/// <summary>
/// 調査で達成したこと（再起動しても消えない）
/// </summary>
public enum ProgressFlag
{
    /// <summary>Memoのロックを解除した</summary>
    MemoUnlocked,

    /// <summary>Messagingの隠された履歴を開いた</summary>
    HiddenHistoryUnlocked,

    /// <summary>鳥の裏側の数字を見つけた</summary>
    BirdNumberFound,

    /// <summary>妻からロッカーの話を聞いた</summary>
    LockerHintReceived,

    /// <summary>ロッカーを開けた</summary>
    LockerOpened
}

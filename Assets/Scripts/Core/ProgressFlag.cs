/// <summary>
/// 調査で達成したこと（電源が落ちても、ページを開き直しても残る）
/// </summary>
public enum ProgressFlag
{
    /// <summary>Memo のロックを解除した</summary>
    MemoUnlocked,

    /// <summary>Messaging の隠された履歴を開いた</summary>
    HiddenHistoryUnlocked,

    /// <summary>鳥がくわえた手紙の裏の数字を見た</summary>
    LetterNumberFound,

    /// <summary>妻からロッカーの話を聞いた（「番号を送る」が出る）</summary>
    LockerHintReceived,

    /// <summary>Messaging を一度開いた（ホームの未読の印が消える）</summary>
    MessagingSeen,

    /// <summary>エンディングにたどり着いた</summary>
    Ended
}

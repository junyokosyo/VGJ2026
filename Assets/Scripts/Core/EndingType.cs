/// <summary>
/// エンディングの種類（仕様書 v0.2：3種類。Bird End は廃止）
/// </summary>
public enum EndingType
{
    /// <summary>まだエンディングに入っていない</summary>
    None,

    /// <summary>隠された履歴を開く前に「何も分からなかった。」を選んだ</summary>
    Normal,

    /// <summary>隠された履歴を開いたが、違う番号を妻に送った</summary>
    Memo,

    /// <summary>隠された履歴を開き、手紙の裏の正しい番号を妻に送った</summary>
    True
}

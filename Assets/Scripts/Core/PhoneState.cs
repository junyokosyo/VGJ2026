/// <summary>
/// スマホの電源の状態
/// </summary>
public enum PhoneState
{
    /// <summary>電源が切れている（電源ボタン待ち）</summary>
    PoweredOff,

    /// <summary>起動の演出中</summary>
    Booting,

    /// <summary>起動中で操作できる（30秒が進む）</summary>
    Running,

    /// <summary>電源が落ちる演出中</summary>
    PoweringOff,

    /// <summary>調査を終えてエンディングへ向かっている</summary>
    Ended
}

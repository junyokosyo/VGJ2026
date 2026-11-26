using System;
using System.Collections.Generic;

/// <summary>
/// 1プレイ分の進行状況を保持する。
/// ・やり直し（電池切れで再起動）：消えない
/// ・リセット（新しくゲームを始める）：Reset で消える
/// どのスクリプトからでも GameProgress.SetFlag(...) のように直接呼べる。
/// </summary>
public static class GameProgress
{
    private static readonly HashSet<ProgressFlag> _flags = new();
    private static readonly HashSet<string> _readMessageIds = new();

    /// <summary>
    /// フラグが新しく立ったとき（引数：立ったフラグ）。
    /// static なイベントなので、登録した側は OnDestroy で必ず解除すること
    /// </summary>
    public static event Action<ProgressFlag> OnFlagSet;

    /// <summary>
    /// リセットされたとき（新しいゲームが始まったとき）。
    /// 暗号の抽選など「リセットのときだけやること」はこれを受け取って行う。
    /// static なイベントなので、登録した側は OnDestroy で必ず解除すること
    /// </summary>
    public static event Action OnReset;

    /// <summary>
    /// 今回のプレイで何回起動したか（1回目の起動で1）
    /// </summary>
    public static int BootCount { get; private set; }

    /// <summary>
    /// 到達したエンディング（OutGame シーンで表示に使う）
    /// </summary>
    public static EndingType Ending { get; private set; }

    /// <summary>
    /// 進行状況をすべて消す。新しくゲームを始めるときに GameManager が呼ぶ
    /// </summary>
    public static void Reset()
    {
        _flags.Clear();
        _readMessageIds.Clear();
        BootCount = 0;
        Ending = EndingType.Normal;
        OnReset?.Invoke();
    }

    /// <summary>
    /// フラグを立てる。すでに立っていれば何もしない
    /// </summary>
    public static void SetFlag(ProgressFlag flag)
    {
        if (_flags.Add(flag))
        {
            OnFlagSet?.Invoke(flag);
        }
    }

    /// <summary>
    /// フラグが立っているか
    /// </summary>
    public static bool HasFlag(ProgressFlag flag)
    {
        return _flags.Contains(flag);
    }

    /// <summary>
    /// メッセージを読んだことを記録する（id は会話データ側で決める）
    /// </summary>
    public static void MarkMessageRead(string messageId)
    {
        _readMessageIds.Add(messageId);
    }

    /// <summary>
    /// メッセージを読んだことがあるか
    /// </summary>
    public static bool IsMessageRead(string messageId)
    {
        return _readMessageIds.Contains(messageId);
    }

    /// <summary>
    /// 起動回数を1増やす。GameManager だけが呼ぶ
    /// </summary>
    public static void CountBoot()
    {
        BootCount++;
    }

    /// <summary>
    /// 到達したエンディングを記録する。GameManager だけが呼ぶ
    /// </summary>
    public static void SetEnding(EndingType ending)
    {
        Ending = ending;
    }
}

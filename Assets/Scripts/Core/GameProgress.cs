using System;
using UnityEngine;

/// <summary>
/// 1プレイ分の進行状況を保持し、PlayerPrefs に保存する（仕様書 v0.2 の3-2）。
/// ・やり直し（電池切れ →「もう一度起動する」）：消えない
/// ・ページを開き直す：保存データから読み込んで続きから
/// ・新しいゲーム（「最初からやり直す」「はじめから遊ぶ」）：NewGame で消して、暗証番号を抽選し直す
/// どのスクリプトからでも GameProgress.SetFlag(...) のように直接呼べる。値を変えると自動で保存する
/// </summary>
public static class GameProgress
{
    private const string SaveKey = "nosubject-save";

    private static SaveData _data;

    /// <summary>
    /// フラグが新しく立ったとき（引数：立ったフラグ）。
    /// static なイベントなので、登録した側は OnDestroy で必ず解除すること
    /// </summary>
    public static event Action<ProgressFlag> OnFlagSet;

    /// <summary>
    /// 新しいゲームが始まったとき。暗証番号の抽選はこれを受け取って行い、SetCodes で入れる。
    /// static なイベントなので、登録した側は OnDestroy で必ず解除すること
    /// </summary>
    public static event Action OnReset;

    /// <summary>
    /// その回の暗証番号の元（写真の枚数・メモの数字・手紙の番号）
    /// </summary>
    public static PuzzleCodes Codes => Data.Codes;

    /// <summary>
    /// 再起動した回数（新しいゲームで0。「もう一度起動する」で1増える）
    /// </summary>
    public static int RebootCount => Data.RebootCount;

    /// <summary>
    /// たどり着いたエンディング（まだなら None）
    /// </summary>
    public static EndingType Ending => Enum.TryParse(Data.Ending, out EndingType ending) ? ending : EndingType.None;

    /// <summary>
    /// 妻に送った番号（まだなら空）
    /// </summary>
    public static string SentNumber => Data.SentNumber;

    /// <summary>
    /// メモの並び順（id の並び。a=9月 b=10月 c=11月 d=12月）
    /// </summary>
    public static string[] MemoOrder => (string[])Data.MemoOrder.Clone();

    /// <summary>
    /// Email で開いていたタブ（"inbox" か "drafts"）
    /// </summary>
    public static string EmailTab => Data.EmailTab;

    /// <summary>
    /// 名前を記録したエンディング（まだなら空）
    /// </summary>
    public static string RecordedEnding => Data.RecordedEnding;

    /// <summary>
    /// 読み込み前に触られても困らないよう、必要になった時点で読み込む
    /// </summary>
    private static SaveData Data
    {
        get
        {
            if (_data == null)
            {
                Load();
            }

            return _data;
        }
    }

    /// <summary>
    /// 保存データを読み込む。保存がない・形式が古い・暗証番号が入っていないときは新しいゲームにする。
    /// GameManager が起動時に呼ぶ（呼ばなくても、最初に値を読んだときに自動で読み込む）
    /// </summary>
    public static void Load()
    {
        _data = null;
        string json = PlayerPrefs.GetString(SaveKey, "");

        if (!string.IsNullOrEmpty(json))
        {
            try
            {
                SaveData loaded = JsonUtility.FromJson<SaveData>(json);

                if (loaded != null && loaded.Version == SaveData.CurrentVersion)
                {
                    _data = loaded;
                }
            }
            catch (ArgumentException)
            {
                Debug.LogWarning("[GameProgress] 保存データが壊れていたので、新しいゲームにします");
            }
        }

        if (_data == null)
        {
            NewGame();
            return;
        }

        if (!_data.Codes.IsValid)
        {
            // 前回、抽選が終わる前に閉じられた場合など
            OnReset?.Invoke();
            WarnIfCodesInvalid();
        }
    }

    /// <summary>
    /// 新しいゲームにする（進行状況をすべて消し、OnReset で暗証番号の抽選を頼む）。
    /// 「最初からやり直す」「はじめから遊ぶ」から呼ぶ
    /// </summary>
    public static void NewGame()
    {
        _data = new SaveData();
        Save();
        OnReset?.Invoke();
        WarnIfCodesInvalid();
    }

    /// <summary>
    /// 暗証番号の元を入れる（柴田さんの抽選から呼ぶ）
    /// </summary>
    public static void SetCodes(PuzzleCodes codes)
    {
        if (codes == null || !codes.IsValid)
        {
            Debug.LogError("[GameProgress] 暗証番号が仕様どおりではありません（写真3〜9枚・数字0〜9・手紙4桁・3つのパスコードが別々）");
            return;
        }

        Data.Codes = codes;
        Save();

#if UNITY_EDITOR
        Debug.Log($"[GameProgress] 今回の答え Memo:{codes.MemoPassword} 履歴:{codes.HistoryPassword} ロッカー:{codes.LockerPassword}");
#endif
    }

    /// <summary>
    /// フラグを立てる。すでに立っていれば何もしない
    /// </summary>
    public static void SetFlag(ProgressFlag flag)
    {
        string key = flag.ToString();

        if (Data.Flags.Contains(key))
        {
            return;
        }

        Data.Flags.Add(key);
        Save();
        OnFlagSet?.Invoke(flag);
    }

    /// <summary>
    /// フラグが立っているか
    /// </summary>
    public static bool HasFlag(ProgressFlag flag)
    {
        return Data.Flags.Contains(flag.ToString());
    }

    /// <summary>
    /// 再起動の回数を1増やす（「もう一度起動する」を押したときに GameManager が呼ぶ）
    /// </summary>
    public static void AddReboot()
    {
        Data.RebootCount++;
        Save();
    }

    /// <summary>
    /// メモの並び順を保存する（Memo で並べ替えたときに呼ぶ）
    /// </summary>
    public static void SetMemoOrder(string[] order)
    {
        if (order == null || order.Length != PuzzleCodes.MonthCount)
        {
            return;
        }

        Data.MemoOrder = (string[])order.Clone();
        Save();
    }

    /// <summary>
    /// Email のタブを保存する
    /// </summary>
    public static void SetEmailTab(string tab)
    {
        Data.EmailTab = tab;
        Save();
    }

    /// <summary>
    /// 妻に番号を送ったことと、そのエンディングを記録する（Messaging から呼ぶ）
    /// </summary>
    public static void SetSentNumber(string sentNumber)
    {
        Data.SentNumber = sentNumber;
        SetEnding(EndingJudge.JudgeSentNumber(sentNumber));
    }

    /// <summary>
    /// たどり着いたエンディングを記録する
    /// </summary>
    public static void SetEnding(EndingType ending)
    {
        Data.Ending = ending.ToString();
        Save();
    }

    /// <summary>
    /// 名前を記録したエンディングを覚える（記録画面から呼ぶ）
    /// </summary>
    public static void SetRecordedEnding(EndingType ending)
    {
        Data.RecordedEnding = ending.ToString();
        Save();
    }

    /// <summary>
    /// 今の状態を PlayerPrefs に書き込む（WebGL ではブラウザの中に保存される）
    /// </summary>
    private static void Save()
    {
        PlayerPrefs.SetString(SaveKey, JsonUtility.ToJson(_data));
        PlayerPrefs.Save();
    }

    /// <summary>
    /// 抽選が行われなかったときに知らせる（柴田さんの抽選がまだシーンにないときなど）
    /// </summary>
    private static void WarnIfCodesInvalid()
    {
        if (!_data.Codes.IsValid)
        {
            Debug.LogWarning("[GameProgress] 暗証番号がまだ抽選されていません。OnReset を受け取って SetCodes を呼ぶ仕組みが必要です");
        }
    }
}

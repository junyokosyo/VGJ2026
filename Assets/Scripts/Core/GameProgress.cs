using System;
using UnityEngine;

/// <summary>
/// 1プレイ分の進行状況を保持し、PlayerPrefs に保存する（仕様書 v0.2 の3-2）。
/// ・やり直し（電池切れ →「もう一度起動する」）：消えない
/// ・新しいゲーム（Title から InGame に入ったとき）：NewGame で消して、暗証番号を抽選し直す
/// ・InGame を直接開いたとき（エディタでのテストなど）：保存データの続きから
/// どのスクリプトからでも GameProgress.SetFlag(...) のように直接呼べる。値を変えると自動で保存する
/// </summary>
public static class GameProgress
{
    private const string SaveKey = "nosubject-save";

    private static SaveData _data;
    private static bool _isNewGameRequested;

    /// <summary>
    /// フラグが新しく立ったとき（引数：立ったフラグ）。
    /// static なイベントなので、登録した側は OnDestroy で必ず解除すること
    /// </summary>
    public static event Action<ProgressFlag> OnFlagSet;

    /// <summary>
    /// 新しいゲームが始まったとき（暗証番号はもう抽選済み）。
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
            // 保存データの番号がおかしいとき（古い形式など）は、番号だけ抽選し直す
            _data.Codes = PasswordLottery.Roll();
            Save();
        }
    }

    /// <summary>
    /// 次に InGame が始まったときに、新しいゲームにするよう予約する（Title から InGame へ進むときに呼ぶ）
    /// </summary>
    public static void RequestNewGame()
    {
        _isNewGameRequested = true;
    }

    /// <summary>
    /// 新しいゲームの予約があれば取り消して true を返す（GameManager が InGame の開始時に呼ぶ）
    /// </summary>
    public static bool ConsumeNewGameRequest()
    {
        bool isRequested = _isNewGameRequested;
        _isNewGameRequested = false;
        return isRequested;
    }

    /// <summary>
    /// 新しいゲームにする（進行状況をすべて消し、暗証番号を抽選し直す）。
    /// 通常は Title から InGame に入ったときに GameManager が呼ぶ
    /// </summary>
    public static void NewGame()
    {
        _data = new SaveData();
        _data.Codes = PasswordLottery.Roll();
        Save();
        OnReset?.Invoke();
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
}

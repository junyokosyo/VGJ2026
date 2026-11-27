using DG.Tweening;
using TMPro;
using UnityEngine;

/// <summary>
/// Messaging アプリ（仕様書 v0.2 の5-5、デモの messages / openhist / sendopen を移植）。
/// ・最初は妻のメッセージと、選択肢「何も分からなかった。」（選ぶと Normal End）
/// ・「パスコードで表示」→ 正解で隠された履歴が開く → 戻ると妻からロッカーの話が届く
/// ・「番号を送る」→ 妻の返事 → 正しければ True End、違えば Memo End
/// 妻の「入力中」の待ち時間はデモと同じ（ロッカーの話 1.5秒、返事 1.8秒、結末へ自動で進むまで 2.6秒）
/// </summary>
public class MessagingApp : AppBase
{
    private const float LockerTypingSeconds = 1.5f;
    private const float ReplyTypingSeconds = 1.8f;
    private const float AutoAdvanceSeconds = 2.6f;
    private const string TypingText = "・・・";

    [Header("参照")]
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private MessagingData _data;

    [Header("会話")]
    [SerializeField] private Transform _chatContent;
    [SerializeField] private ChatBubbleView _bubbleTemplate;

    [Header("上の段（隠された履歴の入口）")]
    [SerializeField] private GameObject _vaultLocked;
    [SerializeField] private GameObject _vaultOpen;

    [Header("下の段（そのときに押せるボタン）")]
    [SerializeField] private GameObject _choiceButton;
    [SerializeField] private GameObject _sendOpenButton;
    [SerializeField] private GameObject _endButton;
    [SerializeField] private TextMeshProUGUI _endButtonLabel;

    [Header("隠された履歴の画面")]
    [SerializeField] private GameObject _historyView;
    [SerializeField] private Transform _historyContent;

    [Header("下から出るパスコード入力")]
    [SerializeField] private GameObject _historyPadSheet;
    [SerializeField] private GameObject _sendPadSheet;

    private bool _isLockerTyping;
    private bool _isReplyTyping;
    private Tween _waitTween;

    /// <summary>
    /// 開いたら既読にして、今の状態の会話を出す
    /// </summary>
    protected override void OnOpened()
    {
        GameProgress.SetFlag(ProgressFlag.MessagingSeen);
        CloseSheets();
        _historyView.SetActive(false);
        Refresh();
    }

    /// <summary>
    /// 閉じたら（電源が落ちたときも）待ち時間を取り消す。再起動後は待たずに続きが表示される
    /// </summary>
    protected override void OnClosed()
    {
        _waitTween?.Kill();
        _isLockerTyping = false;
        _isReplyTyping = false;
    }

    /// <summary>
    /// 「パスコードで表示」を押したとき
    /// </summary>
    public void OpenHistoryPad()
    {
        _historyPadSheet.SetActive(true);
    }

    /// <summary>
    /// 「番号を送る」を押したとき
    /// </summary>
    public void OpenSendPad()
    {
        _sendPadSheet.SetActive(true);
    }

    /// <summary>
    /// パスコード入力を閉じる（× ボタンから）
    /// </summary>
    public void CloseSheets()
    {
        _historyPadSheet.SetActive(false);
        _sendPadSheet.SetActive(false);
    }

    /// <summary>
    /// 隠された履歴のパスコードが正解したとき（PasswordPad の On Correct () に設定する）
    /// </summary>
    public void OnHistoryUnlocked()
    {
        CloseSheets();
        GameProgress.SetFlag(ProgressFlag.HiddenHistoryUnlocked);
        GameProgress.SetFlag(ProgressFlag.LockerHintReceived);
        _isLockerTyping = true;
        OpenHistory();
    }

    /// <summary>
    /// 隠された履歴の画面を開く
    /// </summary>
    public void OpenHistory()
    {
        ClearChildren(_historyContent);

        foreach (ChatLine line in _data.History)
        {
            AddBubble(_historyContent, line.Text, line.IsMine);
        }

        _historyView.SetActive(true);
    }

    /// <summary>
    /// 隠された履歴から戻る。はじめて見たあとなら、妻がロッカーの話を入力し始める
    /// </summary>
    public void CloseHistory()
    {
        _historyView.SetActive(false);
        Refresh();

        if (_isLockerTyping)
        {
            Wait(LockerTypingSeconds, () =>
            {
                _isLockerTyping = false;
                Refresh();
            });
        }
    }

    /// <summary>
    /// 選択肢「何も分からなかった。」を選んだとき
    /// </summary>
    public void PickChoice()
    {
        GameProgress.SetEnding(EndingType.Normal);
        StartReply();
    }

    /// <summary>
    /// 番号を送ったとき（送信モードの PasswordPad の On Send () に設定する）
    /// </summary>
    public void SendNumber(string code)
    {
        CloseSheets();
        GameProgress.SetSentNumber(code);
        StartReply();
    }

    /// <summary>
    /// エンディングへ進む（自動で進むほか、「ロッカーの中を見る」「続ける」ボタンからも呼ぶ）
    /// </summary>
    public void GoToEnding()
    {
        _waitTween?.Kill();

        // プレハブにすると、シーンの GameManager への参照が外れるので、空ならシーンから探す
        if (_gameManager == null)
        {
            _gameManager = FindAnyObjectByType<GameManager>();
        }

        _gameManager.EndGame(GameProgress.Ending);
    }

    /// <summary>
    /// 妻が返事を入力 → 返事を表示 → 少し待ってエンディングへ
    /// </summary>
    private void StartReply()
    {
        _isReplyTyping = true;
        Refresh();

        Wait(ReplyTypingSeconds, () =>
        {
            _isReplyTyping = false;
            Refresh();
            Wait(AutoAdvanceSeconds, GoToEnding);
        });
    }

    /// <summary>
    /// 指定の秒数のあとに処理をする（前の待ちは取り消す）
    /// </summary>
    private void Wait(float seconds, TweenCallback callback)
    {
        _waitTween?.Kill();
        _waitTween = DOVirtual.DelayedCall(seconds, callback).SetLink(gameObject);
    }

    /// <summary>
    /// 今の状態に合わせて、会話と押せるボタンを出し直す
    /// </summary>
    private void Refresh()
    {
        bool hasHistory = GameProgress.HasFlag(ProgressFlag.HiddenHistoryUnlocked);
        bool hasLockerHint = GameProgress.HasFlag(ProgressFlag.LockerHintReceived);
        EndingType ending = GameProgress.Ending;

        _vaultLocked.SetActive(!hasHistory);
        _vaultOpen.SetActive(hasHistory);

        // 会話（上から順に）
        ClearChildren(_chatContent);

        foreach (string text in _data.Today)
        {
            AddBubble(_chatContent, text, false);
        }

        if (ending == EndingType.Normal)
        {
            AddBubble(_chatContent, _data.Choice, true);
        }

        if (hasLockerHint)
        {
            AddBubble(_chatContent, _isLockerTyping ? TypingText : string.Join("\n", _data.Locker), false);
        }

        if (GameProgress.SentNumber != "")
        {
            AddBubble(_chatContent, GameProgress.SentNumber, true);
        }

        if (ending != EndingType.None)
        {
            if (_isReplyTyping)
            {
                AddBubble(_chatContent, TypingText, false);
            }
            else
            {
                foreach (string text in GetReplies(ending))
                {
                    AddBubble(_chatContent, text, false);
                }
            }
        }

        // 下の段のボタン（そのときに押せるものを1つだけ出す）
        _choiceButton.SetActive(!hasHistory && ending == EndingType.None);
        _sendOpenButton.SetActive(hasLockerHint && !_isLockerTyping && ending == EndingType.None);
        _endButton.SetActive(ending != EndingType.None && !_isReplyTyping);
        _endButtonLabel.text = ending == EndingType.True ? _data.ToOpenButton : _data.ToEndButton;
    }

    /// <summary>
    /// エンディングの種類に合った妻の返事
    /// </summary>
    private System.Collections.Generic.List<string> GetReplies(EndingType ending)
    {
        switch (ending)
        {
            case EndingType.True:
                return _data.OpenedReplies;
            case EndingType.Memo:
                return _data.FailedReplies;
            default:
                return _data.ChoiceReplies;
        }
    }

    /// <summary>
    /// 吹き出しを1つ足す（ひな形を複製する）
    /// </summary>
    private void AddBubble(Transform parent, string text, bool isMine)
    {
        ChatBubbleView bubble = Instantiate(_bubbleTemplate, parent);
        bubble.gameObject.SetActive(true);
        bubble.Show(text, isMine);
    }

    /// <summary>
    /// 中身をすべて消す
    /// </summary>
    private void ClearChildren(Transform parent)
    {
        foreach (Transform child in parent)
        {
            Destroy(child.gameObject);
        }
    }
}

using DG.Tweening;
using TMPro;
using UnityEngine;

/// <summary>
/// Email アプリ（仕様書 v0.2 の5-1）。
/// 受信トレイと下書きのタブを切り替えられる（選んだタブは保存する）。
/// 受信メールは開けず「本文は表示できません」と出る。下書きは本文を読める（下書き2が最初の謎のヒント）
/// </summary>
public class EmailApp : AppBase
{
    private const string InboxTab = "inbox";
    private const string DraftsTab = "drafts";
    private const float ToastDuration = 1.9f;

    [Header("データ")]
    [SerializeField] private EmailData _data;

    [Header("一覧")]
    [SerializeField] private GameObject _inboxList;
    [SerializeField] private GameObject _draftsList;
    [SerializeField] private EmailRowView[] _inboxRows;
    [SerializeField] private EmailRowView[] _draftRows;

    [Header("下書きの本文の画面")]
    [SerializeField] private GameObject _draftView;
    [SerializeField] private TextMeshProUGUI _draftSubjectText;
    [SerializeField] private TextMeshProUGUI _draftBodyText;

    [Header("受信メールを押したときのお知らせ")]
    [SerializeField] private GameObject _toast;
    [SerializeField] private TextMeshProUGUI _toastText;

    private Tween _toastTween;

    /// <summary>
    /// 開いたら、前に選んでいたタブの一覧を出す
    /// </summary>
    protected override void OnOpened()
    {
        FillRows();
        _draftView.SetActive(false);
        _toast.SetActive(false);
        ShowTab(GameProgress.EmailTab);
    }

    /// <summary>
    /// 受信トレイのタブを押したとき
    /// </summary>
    public void ShowInbox()
    {
        ShowTab(InboxTab);
    }

    /// <summary>
    /// 下書きのタブを押したとき
    /// </summary>
    public void ShowDrafts()
    {
        ShowTab(DraftsTab);
    }

    /// <summary>
    /// 受信メールを押したとき（本文は開けないので、お知らせを少しのあいだ出す）
    /// </summary>
    public void OpenInbox(int index)
    {
        _toastText.text = _data.CannotOpenText;
        _toast.SetActive(true);

        // 続けて押されたときは、前の「消す」予約を取り消して出し直す
        _toastTween?.Kill();
        _toastTween = DOVirtual.DelayedCall(ToastDuration, () => _toast.SetActive(false)).SetLink(gameObject);
    }

    /// <summary>
    /// 下書きを押したとき（本文の画面を開く）
    /// </summary>
    public void OpenDraft(int index)
    {
        DraftMail draft = _data.DraftMails[index];
        _draftSubjectText.text = draft.Subject;
        _draftBodyText.text = draft.IsCut ? draft.Body + "|" : draft.Body;
        _draftView.SetActive(true);
    }

    /// <summary>
    /// 下書きの本文の画面から一覧へ戻る
    /// </summary>
    public void CloseDraft()
    {
        _draftView.SetActive(false);
    }

    /// <summary>
    /// タブを切り替えて、選んだタブを保存する
    /// </summary>
    private void ShowTab(string tab)
    {
        bool isInbox = tab != DraftsTab;
        _inboxList.SetActive(isInbox);
        _draftsList.SetActive(!isInbox);
        GameProgress.SetEmailTab(isInbox ? InboxTab : DraftsTab);
    }

    /// <summary>
    /// 一覧の各行に、データの文章を入れる
    /// </summary>
    private void FillRows()
    {
        for (int i = 0; i < _inboxRows.Length && i < _data.InboxMails.Count; i++)
        {
            InboxMail mail = _data.InboxMails[i];
            _inboxRows[i].Show(mail.From, mail.Subject);
        }

        for (int i = 0; i < _draftRows.Length && i < _data.DraftMails.Count; i++)
        {
            DraftMail draft = _data.DraftMails[i];
            string firstLine = draft.Body.Split('\n')[0];
            _draftRows[i].Show("下書き　" + draft.Subject, firstLine);
        }
    }
}

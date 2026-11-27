using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Result シーン（デモ v0.27 の流れ）。スマホの画面の中に、エンディングの画面を順に出す。
///
/// TRUE END：
///   ロッカーが開いた →「手紙をひらく」→ 芝田さんの手紙（スマホは隠れる）→ 手紙を閉じる →
///   結びの言葉（スマホが戻る。芝田さんの ExitButton がスマホを出す）→「スマートフォンを閉じる」→ 記録
/// MEMO END・NORMAL END：
///   暗い終わりの画面 →「スマートフォンを閉じる」→ 記録
/// 記録：「はじめから遊ぶ」で Title へ戻る（次に始めたときに進行状況が消える）
///
/// BGM：TRUE END 以外はすぐにエンディングの BGM。TRUE END は手紙を開くまで無音で、開いたら専用の BGM。
/// 「スマートフォンを閉じる」で 2 秒かけて消える。
/// 画面の部品は Tools → NoSubject → Result を v0.27 に合わせる で作ってつなぐ
/// </summary>
public class ResultController : MonoBehaviour
{
    private const float ClosePhoneFadeSeconds = 2f;

    [Header("データ")]
    [SerializeField] private EndingData _data;

    [Header("スマホ（エンディングの画面はこの中に出る）")]
    [SerializeField] private CanvasGroup _phone;

    [Header("ロッカーが開いた画面（TRUE END）")]
    [SerializeField] private CanvasGroup _lockerOpen;
    [SerializeField] private TextMeshProUGUI _lockerPlaceText;
    [SerializeField] private TextMeshProUGUI _lockerTitleText;
    [SerializeField] private TextMeshProUGUI _lockerInsideText;
    [SerializeField] private TextMeshProUGUI _lockerInsideSubText;
    [SerializeField] private Button _openLetterButton;

    [Header("芝田さんの手紙（TRUE END）")]
    [SerializeField] private GameObject _letter;
    [SerializeField] private TextMeshProUGUI _letterText;

    [Header("手紙の文の、いちばん小さい文字の大きさ（長くて入りきらないときに、ここまで縮める）")]
    [SerializeField] private float _letterMinFontSize = 18f;

    [Header("手紙のあとの、結びの言葉の画面（TRUE END）")]
    [SerializeField] private CanvasGroup _trueEndCard;
    [SerializeField] private TextMeshProUGUI _trueLinesText;
    [SerializeField] private TextMeshProUGUI _trueAfterText;
    [SerializeField] private TextMeshProUGUI _trueLabelText;
    [SerializeField] private Button _trueCloseButton;

    [Header("暗い終わりの画面（MEMO END・NORMAL END）")]
    [SerializeField] private CanvasGroup _darkEnd;
    [SerializeField] private TextMeshProUGUI _darkLabelText;
    [SerializeField] private TextMeshProUGUI _darkLinesText;
    [SerializeField] private Button _darkCloseButton;

    [Header("記録の画面")]
    [SerializeField] private CanvasGroup _record;
    [SerializeField] private TextMeshProUGUI _recordEndingText;
    [SerializeField] private TextMeshProUGUI _recordRebootText;
    [SerializeField] private Button _againButton;

    [Header("画面が切り替わるときのフェードの秒数")]
    [SerializeField] private float _fadeDuration = 0.6f;

    private EndingType _type;
    private CanvasGroup _current;

    /// <summary>
    /// 保存データを読み込む（このシーンから直接再生したときのため）
    /// </summary>
    private void Awake()
    {
        GameProgress.Load();
    }

    /// <summary>
    /// 文字を入れて、エンディングに合う最初の画面を出す
    /// </summary>
    private void Start()
    {
        _type = GameProgress.Ending == EndingType.None ? EndingType.Normal : GameProgress.Ending;
        EndingData.Ending ending = _data.Find(_type);
        GameProgress.SetRecordedEnding(_type);

        HideAll();
        SetTexts(ending);
        AddButtonListeners();

        _phone.alpha = 0f;
        _phone.DOFade(1f, _fadeDuration).SetLink(gameObject);
        _phone.blocksRaycasts = true;

        if (_type == EndingType.True)
        {
            FitLetterText(ending != null ? ending.Letter : string.Empty);
            ShowPanel(_lockerOpen);
        }
        else
        {
            ShowPanel(_darkEnd);
            SoundPlayer.PlayEndingBgm(_type);
        }
    }

    /// <summary>
    /// 「はじめから遊ぶ」：Title へ戻る
    /// </summary>
    public void PlayAgain()
    {
        SceneController.Load(GameScene.Title);
    }

    /// <summary>
    /// 「手紙をひらく」：スマホを隠して、芝田さんの手紙を出す。
    /// スマホが消えきってから、中を結びの言葉の画面に替えて、手紙を出す
    /// （消えている途中に替えると、結びの言葉が一瞬見えてしまう）。
    /// 手紙を閉じると、芝田さんの ExitButton がスマホを出すので、そのまま結びの言葉が見える
    /// </summary>
    private void OpenLetter()
    {
        _phone.blocksRaycasts = false;
        _phone.DOKill();
        _phone.DOFade(0f, _fadeDuration).SetLink(gameObject).OnComplete(() =>
        {
            _lockerOpen.alpha = 0f;
            _lockerOpen.blocksRaycasts = false;
            _trueEndCard.alpha = 1f;
            _trueEndCard.blocksRaycasts = true;
            _current = _trueEndCard;

            PlaceLetter();
            PlayBgmWhenLetterOpened();
            _letter.SetActive(true);
        });
    }

    /// <summary>
    /// 「スマートフォンを閉じる」：BGM を消して、記録の画面に替える
    /// </summary>
    private void ClosePhone()
    {
        SoundPlayer.FadeOutBgm(ClosePhoneFadeSeconds);
        ShowPanel(_record);
    }

    /// <summary>
    /// ボタンを押したときの処理をつなぐ
    /// </summary>
    private void AddButtonListeners()
    {
        _openLetterButton.onClick.AddListener(OpenLetter);
        _trueCloseButton.onClick.AddListener(ClosePhone);
        _darkCloseButton.onClick.AddListener(ClosePhone);
        _againButton.onClick.AddListener(PlayAgain);
    }

    /// <summary>
    /// エンディングの文字を、画面ごとに入れる
    /// </summary>
    private void SetTexts(EndingData.Ending ending)
    {
        string label = ending != null ? ending.Label : _type.ToString();
        string lines = ending != null ? ending.Lines : string.Empty;

        _lockerPlaceText.text = _data.LockerPlace;
        _lockerTitleText.text = _data.LockerTitle;
        _lockerInsideText.text = _data.LockerInside;
        _lockerInsideSubText.text = _data.LockerInsideSub;
        SetButtonLabel(_openLetterButton, _data.OpenLetter);

        _trueLinesText.text = lines;
        _trueAfterText.text = ending != null ? ending.After : string.Empty;
        _trueLabelText.text = label;
        SetButtonLabel(_trueCloseButton, _data.ClosePhone);

        _darkLabelText.text = label;
        _darkLinesText.text = lines;
        SetButtonLabel(_darkCloseButton, _data.ClosePhone);

        _recordEndingText.text = label;
        _recordRebootText.text = string.Format(_data.RecordFormat, GameProgress.RebootCount);
        SetButtonLabel(_againButton, _data.Again);
    }

    /// <summary>
    /// ボタンの中の文字を変える
    /// </summary>
    private static void SetButtonLabel(Button button, string label)
    {
        TMP_Text text = button.GetComponentInChildren<TMP_Text>(true);

        if (text != null)
        {
            text.text = label;
        }
    }

    // ───────── 画面の出し入れ ─────────

    /// <summary>
    /// スマホの中の画面を全部隠し、手紙もしまう
    /// </summary>
    private void HideAll()
    {
        foreach (CanvasGroup panel in new[] { _lockerOpen, _trueEndCard, _darkEnd, _record })
        {
            panel.alpha = 0f;
            panel.blocksRaycasts = false;
        }

        _letter.SetActive(false);
        _current = null;
    }

    /// <summary>
    /// 今の画面を消して、次の画面をフェードで出す
    /// </summary>
    private void ShowPanel(CanvasGroup next)
    {
        if (_current != null)
        {
            _current.DOKill();
            _current.DOFade(0f, _fadeDuration).SetLink(gameObject);
            _current.blocksRaycasts = false;
        }

        next.DOKill();
        next.DOFade(1f, _fadeDuration).SetLink(gameObject);
        next.blocksRaycasts = true;
        _current = next;
    }

    // ───────── 芝田さんの手紙 ─────────

    /// <summary>
    /// 手紙（芝田さんの LetterButton）を開いたら、TRUE END の BGM を鳴らす（1回だけ）
    /// </summary>
    private void PlayBgmWhenLetterOpened()
    {
        LetterButton letterButton = _letter.GetComponentInChildren<LetterButton>(true);

        if (letterButton == null)
        {
            SoundPlayer.PlayEndingBgm(_type);
            return;
        }

        Button button = letterButton.GetComponent<Button>();
        UnityEngine.Events.UnityAction play = null;
        play = () =>
        {
            button.onClick.RemoveListener(play);
            SoundPlayer.PlayEndingBgm(_type);
        };
        button.onClick.AddListener(play);
    }

    /// <summary>
    /// 手紙の文を入れて、便箋の枠からはみ出さない大きさに自動で縮める
    /// （手紙の文は長いので、決まった大きさのままだと「ヒナ」「パパ」やボタンに重なる）
    /// </summary>
    private void FitLetterText(string letter)
    {
        _letterText.enableAutoSizing = true;
        _letterText.fontSizeMax = _letterText.fontSize;
        _letterText.fontSizeMin = _letterMinFontSize;
        _letterText.text = letter;
    }

    /// <summary>
    /// 手紙を画面の真ん中に置く（並べる部品があっても、手紙だけは並べる対象から外す）。
    /// 手紙はスマホより奥に置く（手前にあると、閉じたあとの透明な封筒が、スマホのボタンを押したときに反応してしまう）
    /// </summary>
    private void PlaceLetter()
    {
        Transform letter = _letter.transform;
        Transform phone = _phone.transform;

        if (letter.parent == phone.parent && letter.GetSiblingIndex() > phone.GetSiblingIndex())
        {
            letter.SetSiblingIndex(phone.GetSiblingIndex());
        }

        if (!_letter.TryGetComponent(out LayoutElement layout))
        {
            layout = _letter.AddComponent<LayoutElement>();
        }

        layout.ignoreLayout = true;

        RectTransform rect = (RectTransform)_letter.transform;
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = Vector2.zero;
    }
}

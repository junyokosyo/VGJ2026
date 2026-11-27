using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Result シーン（簡易版）。たどり着いたエンディングの文章と、再起動の回数を出す。
/// TRUE END のときは手紙も出す。「はじめから遊ぶ」で Title へ戻る（次に始めたときに進行状況が消える）
/// </summary>
public class ResultController : MonoBehaviour
{
    [Header("データ")]
    [SerializeField] private EndingData _data;

    [Header("表示")]
    [SerializeField] private CanvasGroup _content;
    [SerializeField] private TextMeshProUGUI _labelText;
    [SerializeField] private TextMeshProUGUI _linesText;
    [SerializeField] private TextMeshProUGUI _titleText;
    [SerializeField] private TextMeshProUGUI _rebootText;

    [Header("TRUE END の手紙")]
    [SerializeField] private GameObject _letter;
    [SerializeField] private TextMeshProUGUI _letterText;

    [Header("手紙の文の、いちばん小さい文字の大きさ（長くて入りきらないときに、ここまで縮める）")]
    [SerializeField] private float _letterMinFontSize = 18f;

    [Header("結果の文字のまとまり（手紙の ExitButton が最初に透明にするもの）")]
    [SerializeField] private CanvasGroup _texts;

    [Header("出てくるときのフェードの秒数")]
    [SerializeField] private float _fadeDuration = 1.2f;

    /// <summary>
    /// 保存データを読み込む（このシーンから直接再生したときのため）
    /// </summary>
    private void Awake()
    {
        GameProgress.Load();
    }

    /// <summary>
    /// エンディングの文章を出して、記録に残す
    /// </summary>
    private void Start()
    {
        EndingType type = GameProgress.Ending == EndingType.None ? EndingType.Normal : GameProgress.Ending;
        EndingData.Ending ending = _data.Find(type);

        _labelText.text = ending != null ? ending.Label : type.ToString();
        _linesText.text = ending != null ? ending.Lines : string.Empty;
        _titleText.text = _data.Title;
        _rebootText.text = string.Format(_data.RebootFormat, GameProgress.RebootCount);

        bool hasLetter = ending != null && !string.IsNullOrEmpty(ending.Letter);
        _letter.SetActive(hasLetter);

        if (hasLetter)
        {
            FitLetterText(ending.Letter);
            PlaceLetter();
        }
        else
        {
            ShowTexts();
        }

        GameProgress.SetRecordedEnding(type);

        _content.alpha = 0f;
        _content.DOFade(1f, _fadeDuration).SetLink(gameObject);
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
    /// 手紙を画面の真ん中に置く。
    /// Content は「手紙｜結果の文字」を横に並べる作り（HorizontalLayoutGroup）なので、
    /// そのままだと手紙が並べる枠に押し込まれてずれる。手紙だけ並べる対象から外す
    /// </summary>
    private void PlaceLetter()
    {
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

    /// <summary>
    /// 結果の文字を見える・押せるようにする。
    /// 手紙の ExitButton は、起動したとき（Awake）に結果の文字を透明にする。
    /// 手紙を出さないエンディングでは Exit が押されないので、ここで戻す（戻さないと黒い画面のまま止まる）
    /// </summary>
    private void ShowTexts()
    {
        if (_texts == null)
        {
            return;
        }

        _texts.alpha = 1f;
        _texts.blocksRaycasts = true;
    }

    /// <summary>
    /// 「はじめから遊ぶ」：Title へ戻る
    /// </summary>
    public void PlayAgain()
    {
        SceneController.Load(GameScene.Title);
    }
}

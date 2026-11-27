using DG.Tweening;
using TMPro;
using UnityEngine;

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
            _letterText.text = ending.Letter;
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

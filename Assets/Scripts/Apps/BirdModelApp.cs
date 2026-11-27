using DG.Tweening;
using TMPro;
using UnityEngine;

/// <summary>
/// 3D Bird Model アプリ（仕様書 v0.2 の5-4）。
/// 鳥の模型をドラッグで回して見る。鳥がくわえている手紙の裏がこちらを 0.3 秒向いたら「見つけた」ことにして、
/// 下に「手紙の裏に書かれた数字」を出す（これがロッカーの番号。一度見つけたら、次からは最初から出ている）
/// 手紙の裏の文字はシーンの 3D の置き場にあるので、プレハブにして参照が外れたときは、名前（LetterBack）で探す
/// </summary>
public class BirdModelApp : AppBase
{
    [Header("くるくる回す画面")]
    [SerializeField] private ModelViewer _viewer;

    [Header("手紙の裏（番号の文字。文字の読める面が手紙の裏の面）")]
    [SerializeField] private TextMeshPro _letterNumberText;

    [Header("見つけたあとに出す、下の数字の表示")]
    [SerializeField] private GameObject _readout;
    [SerializeField] private TextMeshProUGUI _readoutNumberText;

    [Header("どれだけ正面を向いたら・何秒向いたら見つけたことにするか")]
    [SerializeField] private float _facingThreshold = 0.75f;
    [SerializeField] private float _foundSeconds = 0.3f;

    private const string LetterBackName = "LetterBack";

    private float _facingTime;

    /// <summary>
    /// 開いたら、向きを戻して番号を書き込み、もう見つけていれば数字を出しておく
    /// </summary>
    protected override void OnOpened()
    {
        FindLetterIfMissing();

        string number = GameProgress.Codes.LetterNumber;
        _letterNumberText.text = string.Join(" ", number.ToCharArray());
        _readoutNumberText.text = number;
        _readout.SetActive(GameProgress.HasFlag(ProgressFlag.LetterNumberFound));

        _viewer.SnapToBase();
        _facingTime = 0f;
    }

    /// <summary>
    /// 手紙の裏の文字が空なら、回している軸の中から名前で探す（プレハブにしたときのため）
    /// </summary>
    private void FindLetterIfMissing()
    {
        if (_letterNumberText != null || _viewer.Pivot == null)
        {
            return;
        }

        foreach (TextMeshPro text in _viewer.Pivot.GetComponentsInChildren<TextMeshPro>(true))
        {
            if (text.name == LetterBackName)
            {
                _letterNumberText = text;
                return;
            }
        }

        Debug.LogError($"[BirdModelApp] 手紙の裏の文字（{LetterBackName}）が見つかりません");
    }

    /// <summary>
    /// 手紙の裏がこちらを向いている時間を数える
    /// </summary>
    private void Update()
    {
        if (_letterNumberText == null || GameProgress.HasFlag(ProgressFlag.LetterNumberFound))
        {
            return;
        }

        if (IsLetterBackFacingCamera())
        {
            _facingTime += Time.deltaTime;

            if (_facingTime >= _foundSeconds)
            {
                Find();
            }
        }
        else
        {
            _facingTime = 0f;
        }
    }

    /// <summary>
    /// 手紙の裏の面が、カメラの方を向いているか。
    /// 文字（TextMeshPro）は transform.forward の反対側から読めるので、その向きとカメラへの向きを比べる
    /// </summary>
    private bool IsLetterBackFacingCamera()
    {
        Transform back = _letterNumberText.transform;
        Vector3 toCamera = (_viewer.ViewCamera.transform.position - back.position).normalized;
        return Vector3.Dot(-back.forward, toCamera) > _facingThreshold;
    }

    /// <summary>
    /// 番号を見つけた：進行状況に残して、下に数字を出す
    /// </summary>
    private void Find()
    {
        GameProgress.SetFlag(ProgressFlag.LetterNumberFound);

        _readout.SetActive(true);
        _readout.transform.localScale = Vector3.one * 0.9f;
        _readout.transform.DOScale(1f, 0.4f).SetEase(Ease.OutBack).SetLink(_readout);
    }
}

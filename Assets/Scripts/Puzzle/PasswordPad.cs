using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// 4桁のパスコード入力（柴田さんの DisplayPasswordText と PasswordChecker をまとめたもの）。
/// 入力パッドごとに1つ置き、どのパスコードと比べるかをインスペクターで選ぶ。
/// 数字ボタンは PasswordInput、消すボタンは On Click () で DeleteDigit を呼ぶ。
/// 送信モード（妻に番号を送る入力）では答え合わせをせず、数字をそのまま表示し、送信ボタン（Send）で送る。
/// 間違えたときは ●●●● が左右に揺れてから消える（揺れているあいだは入力できない）
/// </summary>
public class PasswordPad : MonoBehaviour
{
    private const int PasswordLength = 4;

    [Header("どのパスコードと比べるか")]
    [SerializeField] private PasswordType _type;

    [Header("入力した数を ●○ で表示するテキスト")]
    [SerializeField] private TextMeshProUGUI _display;

    [Header("正解したときに呼ぶ処理（Memo なら MemoApp.Unlock など）")]
    [SerializeField] private UnityEvent _onCorrect;

    [Header("送信モード（妻に番号を送る入力）")]
    [SerializeField] private bool _isSendMode;
    [SerializeField] private Button _sendButton;
    [SerializeField] private UnityEvent<string> _onSend;

    [Header("間違えたときに揺れる秒数と、揺れの大きさ")]
    [SerializeField] private float _shakeDuration = 0.42f;
    [SerializeField] private float _shakeStrength = 9f;

    private string _input = "";
    private bool _isShaking;

    /// <summary>
    /// 表示されるたびに入力を消す（アプリを閉じた・電源が落ちたあとも、まっさらから）
    /// </summary>
    private void OnEnable()
    {
        StopShake();
        Clear();
    }

    /// <summary>
    /// 数字を1つ入れる。4桁そろったら答え合わせをする
    /// </summary>
    public void AddDigit(int digit)
    {
        if (_isShaking || _input.Length >= PasswordLength)
        {
            return;
        }

        _input += digit.ToString();
        SoundPlayer.PlaySe(SeKind.Tap);
        Refresh();

        if (_input.Length == PasswordLength && !_isSendMode)
        {
            Check();
        }
    }

    /// <summary>
    /// 最後の数字を1つ消す
    /// </summary>
    public void DeleteDigit()
    {
        if (_isShaking || _input.Length == 0)
        {
            return;
        }

        _input = _input.Substring(0, _input.Length - 1);
        SoundPlayer.PlaySe(SeKind.Delete);
        Refresh();
    }

    /// <summary>
    /// 送信モードで、4桁そろっていれば送る（送信ボタンの On Click () に設定する）
    /// </summary>
    public void Send()
    {
        if (!_isSendMode || _input.Length < PasswordLength)
        {
            return;
        }

        string code = _input;
        SoundPlayer.PlaySe(SeKind.Send);
        Clear();
        _onSend?.Invoke(code);
    }

    /// <summary>
    /// 入力をすべて消す
    /// </summary>
    public void Clear()
    {
        _input = "";
        Refresh();
    }

    /// <summary>
    /// 答え合わせ。正解なら入力を消して知らせる。間違いなら揺らしてから消す
    /// </summary>
    private void Check()
    {
        if (_input == GetCorrectPassword())
        {
            Clear();
            _onCorrect?.Invoke();
            return;
        }

        Shake();
    }

    /// <summary>
    /// ●●●● を左右に揺らし、終わったら入力を消す
    /// </summary>
    private void Shake()
    {
        _isShaking = true;

        if (_display != null)
        {
            _display.rectTransform.DOPunchAnchorPos(new Vector2(-_shakeStrength, 0f), _shakeDuration, 10, 0.5f).SetLink(gameObject);
        }

        DOVirtual.DelayedCall(_shakeDuration, () =>
        {
            _isShaking = false;
            Clear();
        }).SetLink(gameObject).SetId(this);
    }

    /// <summary>
    /// 揺れを止める（パッドが閉じられたときなど）
    /// </summary>
    private void StopShake()
    {
        DOTween.Kill(this);

        if (_display != null)
        {
            _display.rectTransform.DOComplete();
        }

        _isShaking = false;
    }

    /// <summary>
    /// 今回の正解（新しいゲームのたびに抽選される）
    /// </summary>
    private string GetCorrectPassword()
    {
        switch (_type)
        {
            case PasswordType.Memo:
                return GameProgress.Codes.MemoPassword;
            case PasswordType.History:
                return GameProgress.Codes.HistoryPassword;
            default:
                return GameProgress.Codes.LockerPassword;
        }
    }

    /// <summary>
    /// 表示を更新する。ふだんは入力した数だけ ●（例：●●○○）、送信モードは数字そのもの（例：07__）
    /// </summary>
    private void Refresh()
    {
        if (_sendButton != null)
        {
            _sendButton.interactable = _input.Length == PasswordLength;
        }

        if (_display == null)
        {
            return;
        }

        if (_isSendMode)
        {
            _display.text = _input + new string('_', PasswordLength - _input.Length);
        }
        else
        {
            _display.text = new string('●', _input.Length) + new string('○', PasswordLength - _input.Length);
        }
    }
}

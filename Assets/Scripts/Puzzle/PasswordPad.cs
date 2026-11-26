using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// 4桁のパスコード入力（柴田さんの DisplayPasswordText と PasswordChecker をまとめたもの）。
/// 入力パッドごとに1つ置き、どのパスコードと比べるかをインスペクターで選ぶ。
/// 数字ボタンは PasswordInput、消すボタンは On Click () で DeleteDigit を呼ぶ。
/// 送信モード（妻に番号を送る入力）では答え合わせをせず、数字をそのまま表示し、送信ボタン（Send）で送る
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

    private string _input = "";

    /// <summary>
    /// 表示されるたびに入力を消す（アプリを閉じた・電源が落ちたあとも、まっさらから）
    /// </summary>
    private void OnEnable()
    {
        Clear();
    }

    /// <summary>
    /// 数字を1つ入れる。4桁そろったら答え合わせをする
    /// </summary>
    public void AddDigit(int digit)
    {
        if (_input.Length >= PasswordLength)
        {
            return;
        }

        _input += digit.ToString();
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
        if (_input.Length == 0)
        {
            return;
        }

        _input = _input.Substring(0, _input.Length - 1);
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
    /// 答え合わせ。正解なら知らせる。どちらの場合も入力は消す（不正解の演出はあとで足す）
    /// </summary>
    private void Check()
    {
        bool isCorrect = _input == GetCorrectPassword();
        Clear();

        if (isCorrect)
        {
            _onCorrect?.Invoke();
        }
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

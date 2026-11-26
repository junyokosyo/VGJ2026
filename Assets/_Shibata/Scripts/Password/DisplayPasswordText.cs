using TMPro;
using UnityEngine;

public class DisplayPasswordText : MonoBehaviour
{
    /// <summary>
    /// 自分の回答を表示するテキスト
    /// </summary>
    private TextMeshProUGUI _displayText;
    /// <summary>
    /// 自分の回答
    /// </summary>
    private string _password;
    /// <summary>
    /// 入力できる桁-1した値
    /// </summary>
    private int _max = 3;
    /// <summary>
    /// 正誤の判定するクラス
    /// </summary>
    private PasswordChecker checker = new();

    void Awake()
    {
        _displayText = GetComponent<TextMeshProUGUI>();
        PasswordInput.OnClicked += AddPassword;
        PasswordChecker.OnReset += ResetText;
    }

    private void OnDestroy()
    {
        PasswordInput.OnClicked -= AddPassword;
        PasswordChecker.OnReset -= ResetText;
    }

    /// <summary>
    /// 入力
    /// </summary>
    /// <param name="value">入力した値</param>
    void AddPassword(int value)
    {
        _password += value.ToString();
        _displayText.text = _password;

        if (_displayText.text.Length > _max)
        {
            checker.CheckAnswer(_password);
        }
    }

    /// <summary>
    /// テキストの表示をリセット
    /// </summary>
    void ResetText()
    {
        _password = null;
        _displayText.text = "";
    }
}

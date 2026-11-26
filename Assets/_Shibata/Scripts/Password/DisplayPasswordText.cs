using TMPro;
using UnityEngine;

public class DisplayPasswordText : MonoBehaviour
{
    private TextMeshProUGUI _displayText;
    private string _password;
    private int _max = 3;
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

    void AddPassword(int value)
    {
        _password += value.ToString();
        _displayText.text = _password;

        if (_displayText.text.Length > _max)
        {
            checker.CheckAnswer(_password);
        }
    }

    void ResetText()
    {
        _password = null;
        _displayText.text = "";
    }
}

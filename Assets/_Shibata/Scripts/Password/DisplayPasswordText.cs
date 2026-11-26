using TMPro;
using UnityEngine;

public class DisplayPasswordText : MonoBehaviour
{
    private TextMeshProUGUI _text;
    private string _password;
    private int _max = 4;

    void Awake()
    {
        _text = GetComponent<TextMeshProUGUI>();
        PasswordInput.OnClicked += AddPassword;
    }

    private void OnDestroy()
    {
        PasswordInput.OnClicked -= AddPassword;
    }

    void AddPassword(int value)
    {
        if (_text.text.Length >= _max)
        {
            //パスワードが合っているか確かめる処理
            return;
        }

        _password += value.ToString();
        _text.text = _password;
    }
}

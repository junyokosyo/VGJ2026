using TMPro;
using UnityEngine;

public class DisplayBirthDay : MonoBehaviour
{
    [SerializeField,Header("誕生日を表示するテキスト")]
    private TextMeshProUGUI _text;

    private void Awake()
    {
        OnDisplay();
    }

    public void OnDisplay()
    {
        _text.text = PasswordManager.Instance.DataBase.LockPasswordAnswers.Birth;
    }
}

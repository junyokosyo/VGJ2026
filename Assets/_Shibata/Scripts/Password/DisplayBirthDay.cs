using TMPro;
using UnityEngine;

public class DisplayBirthDay : MonoBehaviour
{
    [SerializeField,Header("誕生日を表示するテキスト")]
    private TextMeshProUGUI _text;


    private void Start()
    {
        OnDisplay();
    }

    /// <summary>
    /// 誕生日の表示
    /// </summary>
    public void OnDisplay()
    {
        _text.text = PasswordManager.Instance.DataBase.LockPasswordAnswers.Birth;
    }
}
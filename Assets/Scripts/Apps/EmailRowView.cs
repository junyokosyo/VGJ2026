using TMPro;
using UnityEngine;

/// <summary>
/// Email の一覧の1行（差出人・件名）。
/// 押したときの処理は、Button の On Click () で EmailApp.OpenInbox / OpenDraft（何行目か）を呼ぶ
/// </summary>
public class EmailRowView : MonoBehaviour
{
    [Header("参照")]
    [SerializeField] private TextMeshProUGUI _titleText;
    [SerializeField] private TextMeshProUGUI _subText;

    /// <summary>
    /// 表示を更新する（上の行と下の行）
    /// </summary>
    public void Show(string title, string sub)
    {
        _titleText.text = title;
        _subText.text = sub;
    }
}

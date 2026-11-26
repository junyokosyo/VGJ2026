using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 会話の吹き出し1つ。自分の発言は右寄せ・アクセント色、相手の発言は左寄せ・灰色
/// </summary>
public class ChatBubbleView : MonoBehaviour
{
    [Header("参照")]
    [SerializeField] private HorizontalLayoutGroup _row;
    [SerializeField] private Image _bubble;
    [SerializeField] private TextMeshProUGUI _text;

    [Header("色")]
    [SerializeField] private Color _mineColor = new Color(1f, 0.61f, 0.91f);
    [SerializeField] private Color _theirColor = new Color(0.92f, 0.92f, 0.94f);

    /// <summary>
    /// 文字と向きを設定する
    /// </summary>
    public void Show(string text, bool isMine)
    {
        _text.text = text;
        _bubble.color = isMine ? _mineColor : _theirColor;
        _row.childAlignment = isMine ? TextAnchor.UpperRight : TextAnchor.UpperLeft;
    }
}

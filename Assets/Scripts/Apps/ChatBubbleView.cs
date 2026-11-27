using System;
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

    [Header("テキストのサイズ")]
    [SerializeField] private int _maxTextWidth = 440;

    /// <summary>
    /// 文字と向きを設定する
    /// </summary>
    public void Show(string text, bool isMine)
    {
        _text.text = text;
        _bubble.color = isMine ? _mineColor : _theirColor;
        _row.childAlignment = isMine ? TextAnchor.UpperRight : TextAnchor.UpperLeft;

        var textLength = text.Length;
        var fontSize = (int)_text.fontSize;

        int bubbleWidth = (textLength * (int)fontSize) + ((int)fontSize * 2);

        if(bubbleWidth > _maxTextWidth)
        {
            bubbleWidth = _maxTextWidth;
        }

        _bubble.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, bubbleWidth);

        Debug.Log($"本文=[{text}] 文字数={text.Length}");
    }
}

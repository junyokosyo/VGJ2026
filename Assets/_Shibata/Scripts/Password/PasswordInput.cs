using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// パスコード入力の数字ボタン（柴田さんのものを、入力パッドごとに動くように変更）。
/// 押すと、つないだ PasswordPad に自分の数字を入れる（元の OnClicked も呼ぶ）
/// </summary>
[RequireComponent(typeof(Image))]
public class PasswordInput : MonoBehaviour, IPointerDownHandler
{
    /// <summary>
    /// 入力ボタンをクリックした時（柴田さんの元の仕組み。DisplayPasswordText が受け取る）
    /// </summary>
    public static Action<int> OnClicked;

    [Header("このボタンの数字"), Range(0, 9)]
    [SerializeField] private int _id;

    [Header("参照")]
    [SerializeField] private PasswordPad _pad;
    [SerializeField] private AnswerDatabase _dataBase;

    /// <summary>
    /// データベースに自分の数字の画像があれば、ボタンの見た目にする
    /// </summary>
    private void Awake()
    {
        if (_dataBase == null)
        {
            return;
        }

        foreach (ButtonData data in _dataBase.PicturePasswords)
        {
            if (data.ID == _id && data.ImageSprite != null)
            {
                GetComponent<Image>().sprite = data.ImageSprite;
            }
        }
    }

    /// <summary>
    /// 押されたら数字を入れる
    /// </summary>
    public void OnPointerDown(PointerEventData eventData)
    {
        if (_pad != null)
        {
            _pad.AddDigit(_id);
        }

        OnClicked?.Invoke(_id);
    }
}

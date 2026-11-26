using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Image = UnityEngine.UI.Image;

public class PasswordInput : MonoBehaviour, IPointerDownHandler
{
    /// <summary>
    /// 入力ボタンをクリックした時
    /// </summary>
    public static Action<int> OnClicked;

    [SerializeField, Range(0, 9),Header("取得したいID")]
    private int _id;

    /// <summary>
    /// 入力ボタンのデータの取得
    /// </summary>
    private ButtonData _picturePassword;
    private Image _image;

    public void OnPointerDown(PointerEventData data)
    {
        OnClicked?.Invoke(_id);
    }

    void Awake()
    {
        foreach (var data in PasswordManager.Instance.DataBase.PicturePasswords)
        {
            if (data.ID == _id)
            {
                _picturePassword = data;
            }
        }

        _image = GetComponent<Image>();
        _image.sprite = _picturePassword.ImageSprite;
    }
}

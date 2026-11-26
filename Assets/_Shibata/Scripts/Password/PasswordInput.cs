using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Image = UnityEngine.UI.Image;

public class PasswordInput : MonoBehaviour, IPointerDownHandler
{
    public static Action<int> OnClicked;

    [SerializeField, Range(0, 9)]
    private int _id;
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

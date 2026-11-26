using UnityEngine;
using UnityEngine.EventSystems;
using Image = UnityEngine.UI.Image;

public class PasswordInput : MonoBehaviour,IPointerDownHandler
{
    [SerializeField]
    private AnswerDatabase _answerDatabase;
    [SerializeField]
    private int _id;
    private PicturePassword _picturePassword;
    private Image _image;

    public void OnPointerDown(PointerEventData data)
    {
        Debug.Log(_id);
    }

    void Awake()
    {
        foreach(var data in _answerDatabase.PicturePasswords)
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

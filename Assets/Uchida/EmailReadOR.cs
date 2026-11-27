using UnityEngine;
using UnityEngine.UI;

public class EmailReadOR : MonoBehaviour
{
    [SerializeField] private Button _emailIcon;
    [SerializeField] private Sprite _notReadEmailImage;
    [SerializeField] private Sprite _readEmailImage;
    [SerializeField] private GameObject _emailApp;

    [HideInInspector]
    private bool _isEmailRead = false;

    private void Update()
    {
        if (_emailApp.activeSelf)
        {
            _isEmailRead = true;
        }
        if (!_isEmailRead)
        {
            _emailIcon.image.sprite = _notReadEmailImage;
        }
        else
        {
            _emailIcon.image.sprite = _readEmailImage;
        }
    }
}

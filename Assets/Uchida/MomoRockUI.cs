using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ƒƒ‚‚Ì“ä‚ğ‰ğ‚¢‚½‚©‚Ç‚¤‚©‚ğ”»’è‚µƒƒbƒN‚ğON/OFF‚·‚é
/// </summary>
public class MomoRockUI : MonoBehaviour
{
    [SerializeField] private PasswordPad _passwordPad;
    [SerializeField] private Button _memoIcon;
    [SerializeField] private Image _memoRock;

    private void Update()
    {
        if (_passwordPad.IsClear == true && _memoIcon)
        {
            _memoRock.gameObject.SetActive(false);
        }
        else if (_passwordPad.IsClear == false && _memoIcon)
        {
            _memoRock.gameObject.SetActive(true);
        }
    }
}

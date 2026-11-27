using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class LetterButton : MonoBehaviour
{
    [SerializeField]
    private CanvasGroup _contentsLatters;
    private CanvasGroup _latter;
    private Button _button;
    private void Awake()
    {
        _button = GetComponent<Button>();
        _latter = GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        _button.onClick.AddListener(OnOpen);

        _contentsLatters.alpha = 0;
    }

    private void OnDestroy()
    {
        _button.onClick.RemoveListener(OnOpen);
    }

    void OnOpen()
    {
        _latter.DOFade(0, 1);
        _contentsLatters.DOFade(1, 1);
    }
}

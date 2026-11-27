using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ExitButton : MonoBehaviour
{
    [SerializeField]
    private CanvasGroup TextsGroup;
    [SerializeField]
    private CanvasGroup parentCanvasGroup;
    private Button button;

    void Awake()
    {
        button = GetComponent<Button>();
        TextsGroup.alpha = 0;
        TextsGroup.blocksRaycasts = false;
    }

    void Start()
    {
        button.onClick.AddListener(OnExit);
    }

    private void OnDestroy()
    {
        button.onClick.RemoveListener(OnExit);
    }

    void OnExit()
    {
        parentCanvasGroup.DOFade(0, 1);
        TextsGroup.DOFade(1, 1).OnComplete(() => TextsGroup.blocksRaycasts = true) ;
    }
}

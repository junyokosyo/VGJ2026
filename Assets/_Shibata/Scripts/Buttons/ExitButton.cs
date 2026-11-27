using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ExitButton : MonoBehaviour
{
    [SerializeField]
    private CanvasGroup canvasGroup;
    private Button button;

    void Awake()
    {
        button = GetComponent<Button>();
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
        canvasGroup.DOFade(0, 1);
    }
}

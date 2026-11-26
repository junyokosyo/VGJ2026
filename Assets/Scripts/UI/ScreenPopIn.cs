using DG.Tweening;
using UnityEngine;

/// <summary>
/// 画面が出るたびに、少し大きい状態から縮みながらふわっと出す（iPhone のロック解除やアプリを閉じたときの動き）。
/// ホーム画面などに付ける
/// </summary>
[RequireComponent(typeof(CanvasGroup))]
public class ScreenPopIn : MonoBehaviour
{
    [Header("最初の大きさと、出てくるまでの秒数")]
    [SerializeField] private float _startScale = 1.08f;
    [SerializeField] private float _duration = 0.35f;

    private CanvasGroup _canvasGroup;

    /// <summary>
    /// 自分の CanvasGroup を覚える
    /// </summary>
    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    /// <summary>
    /// 出てくるたびに動かす
    /// </summary>
    private void OnEnable()
    {
        transform.DOKill();
        _canvasGroup.DOKill();

        transform.localScale = Vector3.one * _startScale;
        _canvasGroup.alpha = 0f;

        transform.DOScale(1f, _duration).SetEase(Ease.OutCubic).SetLink(gameObject);
        _canvasGroup.DOFade(1f, _duration).SetEase(Ease.OutCubic).SetLink(gameObject);
    }
}

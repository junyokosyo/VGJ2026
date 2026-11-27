using DG.Tweening;
using UnityEngine;

/// <summary>
/// 表示されているあいだ、ずっとくり返す小さな動き（デモの float・notifglow と同じ）。
/// ・濃さ：_fromAlpha と _toAlpha のあいだを行ったり来たりする（ピンクにゆっくり光る、など）
/// ・上下：_moveY だけ上下にふわふわ動く（0 なら動かない）
/// 付けたオブジェクトの CanvasGroup を使うので、子もまとめて動く
/// </summary>
[RequireComponent(typeof(CanvasGroup))]
public class IdleLoop : MonoBehaviour
{
    [Header("濃さ（行ったり来たり）")]
    [SerializeField] private float _fromAlpha = 0.55f;
    [SerializeField] private float _toAlpha = 1f;

    [Header("上下にふわふわ動く幅（0 なら動かない）")]
    [SerializeField] private float _moveY = 0f;

    [Header("片道の秒数（行って戻るまでは、この2倍）")]
    [SerializeField] private float _halfCycle = 0.9f;

    private CanvasGroup _canvasGroup;
    private RectTransform _rect;
    private Vector2 _basePosition;

    /// <summary>
    /// 自分の部品と、元の位置を覚える
    /// </summary>
    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _rect = (RectTransform)transform;
        _basePosition = _rect.anchoredPosition;
    }

    /// <summary>
    /// 出てきたら動かし始める
    /// </summary>
    private void OnEnable()
    {
        _canvasGroup.alpha = _fromAlpha;
        _canvasGroup.DOFade(_toAlpha, _halfCycle).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo).SetLink(gameObject);

        if (_moveY != 0f)
        {
            _rect.anchoredPosition = _basePosition - new Vector2(0f, _moveY / 2f);
            _rect.DOAnchorPosY(_basePosition.y + _moveY / 2f, _halfCycle).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo).SetLink(gameObject);
        }
    }

    /// <summary>
    /// 隠れたら止めて、元の位置に戻す
    /// </summary>
    private void OnDisable()
    {
        _canvasGroup.DOKill();
        _rect.DOKill();
        _rect.anchoredPosition = _basePosition;
    }
}

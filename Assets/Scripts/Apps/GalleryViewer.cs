using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Gallery の写真を大きく見る画面（デモ v0.27 と同じ。猪瀬さんの SelectPhoto の考え方を、Gallery の中に組み込んだもの）。
/// ・押した写真を大きく出す。上に月（9月など）、下に説明文
/// ・左右の矢印か、左右に払う（スワイプ）と、前後の写真へ（4か月を通した順番）。最初と最後では矢印を隠す
/// ・「‹ Gallery」で閉じる
/// 写真の一覧は GalleryApp が並べ直すたびに SetPhotos で渡す
/// </summary>
[RequireComponent(typeof(CanvasGroup))]
public class GalleryViewer : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private const float SwipeDistance = 50f;

    [Header("表示")]
    [SerializeField] private Image _photo;
    [SerializeField] private RectTransform _frame;
    [SerializeField] private TextMeshProUGUI _monthText;
    [SerializeField] private TextMeshProUGUI _captionText;
    [SerializeField] private GameObject _prevButton;
    [SerializeField] private GameObject _nextButton;

    [Header("開くときの演出")]
    [SerializeField] private float _openDuration = 0.2f;
    [SerializeField] private float _openScale = 0.92f;

    private readonly List<Sprite> _photos = new();
    private readonly List<int> _months = new();
    private readonly List<string> _captions = new();

    private CanvasGroup _canvasGroup;
    private Canvas _canvas;
    private int _index;
    private Vector2 _dragStart;

    /// <summary>
    /// 自分の部品を覚える
    /// </summary>
    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _canvas = GetComponentInParent<Canvas>();
    }

    /// <summary>
    /// 写真の一覧を受け取る（写真・その月・説明文。説明文がない写真は空）
    /// </summary>
    public void SetPhotos(List<Sprite> photos, List<int> months, List<string> captions)
    {
        _photos.Clear();
        _photos.AddRange(photos);
        _months.Clear();
        _months.AddRange(months);
        _captions.Clear();
        _captions.AddRange(captions);
    }

    /// <summary>
    /// index 番目の写真を大きく出す（ふわっと出る）
    /// </summary>
    public void Open(int index)
    {
        if (_photos.Count == 0)
        {
            return;
        }

        gameObject.SetActive(true);
        Show(index);

        _canvasGroup.DOKill();
        _canvasGroup.alpha = 0f;
        _canvasGroup.DOFade(1f, _openDuration).SetLink(gameObject);

        _frame.DOKill();
        _frame.localScale = Vector3.one * _openScale;
        _frame.DOScale(1f, _openDuration).SetEase(Ease.OutCubic).SetLink(gameObject);
    }

    /// <summary>
    /// 閉じる（「‹ Gallery」・Gallery を開き直したとき）
    /// </summary>
    public void Close()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 次の写真へ（右の矢印・左に払う）
    /// </summary>
    public void ShowNext()
    {
        Show(_index + 1);
    }

    /// <summary>
    /// 前の写真へ（左の矢印・右に払う）
    /// </summary>
    public void ShowPrev()
    {
        Show(_index - 1);
    }

    /// <summary>
    /// 払い始めた位置を覚える
    /// </summary>
    public void OnBeginDrag(PointerEventData eventData)
    {
        _dragStart = eventData.position;
    }

    /// <summary>
    /// 払っているあいだは何もしない（OnBeginDrag・OnEndDrag を受け取るために必要）
    /// </summary>
    public void OnDrag(PointerEventData eventData)
    {
    }

    /// <summary>
    /// 左に払ったら次、右に払ったら前（50 以上動かしたとき）
    /// </summary>
    public void OnEndDrag(PointerEventData eventData)
    {
        float scale = _canvas != null ? _canvas.scaleFactor : 1f;
        float distance = (eventData.position.x - _dragStart.x) / scale;

        if (distance < -SwipeDistance)
        {
            ShowNext();
        }
        else if (distance > SwipeDistance)
        {
            ShowPrev();
        }
    }

    /// <summary>
    /// 写真・月・説明文・左右の矢印を、index 番目に合わせる
    /// </summary>
    private void Show(int index)
    {
        int clamped = Mathf.Clamp(index, 0, _photos.Count - 1);
        bool isChanged = clamped != _index;
        _index = clamped;

        _photo.sprite = _photos[_index];
        _photo.enabled = _photo.sprite != null;
        _monthText.text = $"{_months[_index]}月";
        _captionText.text = _captions[_index];
        _prevButton.SetActive(_index > 0);
        _nextButton.SetActive(_index < _photos.Count - 1);

        // 前後に移ったことが分かるように、写真を少しだけふわっと出す
        if (isChanged && gameObject.activeInHierarchy)
        {
            _photo.DOKill();
            _photo.color = new Color(1f, 1f, 1f, 0.4f);
            _photo.DOFade(1f, 0.15f).SetLink(gameObject);
        }
    }
}

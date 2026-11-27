using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Memo のメモ1枚の表示（タイトルの数字と文章）。
/// 押したときの処理は、Button の On Click () で MemoApp.SelectNote（自分の並び位置 0〜3）を呼ぶ。
/// ドラッグしたときは MemoApp に伝えて、並べ替えてもらう（ドラッグしたときはタップ扱いにならない）
/// </summary>
public class MemoNoteView : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("参照")]
    [SerializeField] private TextMeshProUGUI _numberText;
    [SerializeField] private TextMeshProUGUI _storyText;
    [SerializeField] private Image _background;

    [Header("色")]
    [SerializeField] private Color _normalColor = Color.white;
    [SerializeField] private Color _selectedColor = new Color(1f, 0.85f, 0.95f);
    [SerializeField] private Color _alignedNumberColor = new Color(1f, 0.61f, 0.91f);

    private Color _normalNumberColor;
    private MemoApp _memoApp;
    private int _index = -1;

    /// <summary>
    /// もとの数字の色を覚えておく
    /// </summary>
    private void Awake()
    {
        _normalNumberColor = _numberText.color;
    }

    /// <summary>
    /// 自分の持ち主（MemoApp）と並び位置を覚える（MemoApp が呼ぶ）
    /// </summary>
    public void Init(MemoApp memoApp, int index)
    {
        _memoApp = memoApp;
        _index = index;
    }

    /// <summary>
    /// ドラッグを始めた
    /// </summary>
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (_memoApp != null)
        {
            _memoApp.BeginNoteDrag(_index);
        }
    }

    /// <summary>
    /// ドラッグ中
    /// </summary>
    public void OnDrag(PointerEventData eventData)
    {
        if (_memoApp != null)
        {
            _memoApp.DragNote(eventData.position, eventData.pressEventCamera);
        }
    }

    /// <summary>
    /// ドラッグを終えた
    /// </summary>
    public void OnEndDrag(PointerEventData eventData)
    {
        if (_memoApp != null)
        {
            _memoApp.EndNoteDrag();
        }
    }

    /// <summary>
    /// 表示を更新する
    /// </summary>
    /// <param name="number">タイトルの数字</param>
    /// <param name="story">文章</param>
    /// <param name="isSelected">入れ替えるために選んでいるか・ドラッグ中か</param>
    /// <param name="isAligned">4枚が正しい順に並んでいるか（数字が光る）</param>
    public void Show(int number, string story, bool isSelected, bool isAligned)
    {
        _numberText.text = number.ToString();
        _numberText.color = isAligned ? _alignedNumberColor : _normalNumberColor;
        _storyText.text = story;
        _background.color = isSelected ? _selectedColor : _normalColor;
    }
}

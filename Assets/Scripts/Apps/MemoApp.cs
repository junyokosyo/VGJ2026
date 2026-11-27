using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Memo アプリ（仕様書 v0.2 の5-4）。
/// 最初はパスコードでロックされている。解除すると4枚のメモが出る。
/// 並べ替え：メモをドラッグして上下に動かす（デモ v0.27 と同じ）。2枚を順にタップして入れ替えることもできる。
/// 時系列（9→10→11→12月）に並ぶと、タイトルの数字が光る（その数字が隠された履歴のパスコード）。
/// ドラッグ中は、メモの「中身」が指についてくる（4つの枠は動かさず、中身を入れ替えて見せる）
/// </summary>
public class MemoApp : AppBase
{
    // メモの id（a=9月 b=10月 c=11月 d=12月）。GameProgress のメモの並びはこの id で保存している
    private static readonly string[] NoteIds = { "a", "b", "c", "d" };

    [Header("画面")]
    [SerializeField] private GameObject _lockPanel;
    [SerializeField] private GameObject _notesPanel;

    [Header("メモ4枚（上から順に。並び位置 0〜3）")]
    [SerializeField] private MemoNoteView[] _noteViews = new MemoNoteView[4];

    [Header("参照")]
    [SerializeField] private AnswerDatabase _dataBase;

    private int _selectedIndex = -1;
    private int _dragIndex = -1;

    /// <summary>
    /// メモ4枚に、自分と並び位置を教える（ドラッグを受け取れるようにする）
    /// </summary>
    private void Start()
    {
        for (int i = 0; i < _noteViews.Length; i++)
        {
            _noteViews[i].Init(this, i);
        }
    }

    /// <summary>
    /// 開いたら、ロック中か解除済みかに合わせて表示する
    /// </summary>
    protected override void OnOpened()
    {
        _selectedIndex = -1;
        _dragIndex = -1;
        Refresh();
    }

    /// <summary>
    /// パスコードが正解したときに呼ぶ（PasswordPad の On Correct () に設定する）
    /// </summary>
    public void Unlock()
    {
        GameProgress.SetFlag(ProgressFlag.MemoUnlocked);
        Refresh();
    }

    /// <summary>
    /// メモを押したときに呼ぶ（各メモの Button の On Click () に、並び位置 0〜3 を入れて設定する）。
    /// 1枚目で選び、2枚目で入れ替える
    /// </summary>
    public void SelectNote(int index)
    {
        if (_selectedIndex < 0)
        {
            _selectedIndex = index;
        }
        else
        {
            string[] order = GameProgress.MemoOrder;
            (order[_selectedIndex], order[index]) = (order[index], order[_selectedIndex]);
            GameProgress.SetMemoOrder(order);
            _selectedIndex = -1;
        }

        Refresh();
    }

    /// <summary>
    /// メモのドラッグを始めた（MemoNoteView から呼ばれる）
    /// </summary>
    public void BeginNoteDrag(int index)
    {
        _selectedIndex = -1;
        _dragIndex = index;
        Refresh();
    }

    /// <summary>
    /// ドラッグ中（MemoNoteView から呼ばれる）。指にいちばん近い枠へ、ドラッグ中のメモを動かす
    /// </summary>
    public void DragNote(Vector2 screenPosition, Camera eventCamera)
    {
        if (_dragIndex < 0)
        {
            return;
        }

        int target = FindNearestSlot(screenPosition, eventCamera);

        if (target < 0 || target == _dragIndex)
        {
            return;
        }

        List<string> order = new List<string>(GameProgress.MemoOrder);
        string id = order[_dragIndex];
        order.RemoveAt(_dragIndex);
        order.Insert(target, id);
        GameProgress.SetMemoOrder(order.ToArray());

        _dragIndex = target;
        Refresh();
    }

    /// <summary>
    /// ドラッグを終えた（MemoNoteView から呼ばれる）
    /// </summary>
    public void EndNoteDrag()
    {
        _dragIndex = -1;
        Refresh();
    }

    /// <summary>
    /// 指の位置（縦）にいちばん近いメモの枠の番号（見つからなければ -1）
    /// </summary>
    private int FindNearestSlot(Vector2 screenPosition, Camera eventCamera)
    {
        RectTransform first = (RectTransform)_noteViews[0].transform;

        if (!RectTransformUtility.ScreenPointToWorldPointInRectangle(first, screenPosition, eventCamera, out Vector3 pointer))
        {
            return -1;
        }

        int nearest = -1;
        float nearestDistance = float.MaxValue;

        for (int i = 0; i < _noteViews.Length; i++)
        {
            RectTransform slot = (RectTransform)_noteViews[i].transform;
            float distance = Mathf.Abs(slot.TransformPoint(slot.rect.center).y - pointer.y);

            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearest = i;
            }
        }

        return nearest;
    }

    /// <summary>
    /// 画面を今の状態に合わせる
    /// </summary>
    private void Refresh()
    {
        bool isUnlocked = GameProgress.HasFlag(ProgressFlag.MemoUnlocked);
        _lockPanel.SetActive(!isUnlocked);
        _notesPanel.SetActive(isUnlocked);

        if (!isUnlocked)
        {
            return;
        }

        string[] order = GameProgress.MemoOrder;
        bool isAligned = IsAligned(order);

        for (int i = 0; i < _noteViews.Length; i++)
        {
            int noteIndex = System.Array.IndexOf(NoteIds, order[i]);
            int number = GameProgress.Codes.MemoNumbers[noteIndex];
            string story = GetStory(9 + noteIndex);
            bool isPicked = i == _selectedIndex || i == _dragIndex;
            _noteViews[i].Show(number, story, isPicked, isAligned);
        }
    }

    /// <summary>
    /// 時系列（a→b→c→d）に並んでいるか
    /// </summary>
    private bool IsAligned(string[] order)
    {
        for (int i = 0; i < NoteIds.Length; i++)
        {
            if (order[i] != NoteIds[i])
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// その月のメモの文章（データになければ空）
    /// </summary>
    private string GetStory(int month)
    {
        foreach (Message message in _dataBase.Messages)
        {
            if (message.Month == month)
            {
                return message.Story;
            }
        }

        return "";
    }
}

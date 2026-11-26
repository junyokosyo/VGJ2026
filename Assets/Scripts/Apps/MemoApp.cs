using UnityEngine;

/// <summary>
/// Memo アプリ（仕様書 v0.2 の5-4）。
/// 最初はパスコードでロックされている。解除すると4枚のメモが出て、2枚を順にタップすると入れ替わる。
/// 時系列（9→10→11→12月）に並ぶと、タイトルの数字が光る（その数字が隠された履歴のパスコード）。
/// 並べ替えはドラッグではなくタップで入れ替え（ドラッグはあとで足す）
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

    /// <summary>
    /// 開いたら、ロック中か解除済みかに合わせて表示する
    /// </summary>
    protected override void OnOpened()
    {
        _selectedIndex = -1;
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
            _noteViews[i].Show(number, story, i == _selectedIndex, isAligned);
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

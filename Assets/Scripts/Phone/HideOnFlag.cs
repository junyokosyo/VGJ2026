using UnityEngine;

/// <summary>
/// 進行状況のフラグに合わせて、見た目を出したり消したりする部品。
/// 例：Memo のロックマーク（Flag：MemoUnlocked、Target：ロックマークの Image）
/// ・フラグが立っていなければ Target を出す、立ったら Target を消す（_showWhenSet をオンにすると逆になる）
/// ・ホーム画面が出るたび（OnEnable）と、フラグが立った・新しいゲームになったときに見直す
/// ・フラグは GameProgress に保存されるので、電池が切れて再起動しても状態が残る
/// ・Target 自身には付けない（消えたときに自分も止まって、また出せなくなる）。Target の親など、消えないオブジェクトに付ける
/// </summary>
public class HideOnFlag : MonoBehaviour
{
    [Header("見るフラグ")]
    [SerializeField] private ProgressFlag _flag;

    [Header("出したり消したりするもの")]
    [SerializeField] private GameObject _target;

    [Header("オンにすると、フラグが立ったときに出す（ふだんは、立ったら消す）")]
    [SerializeField] private bool _showWhenSet;

    /// <summary>
    /// 表示されたら今の状態に合わせて、フラグの変化を受け取れるようにする
    /// </summary>
    private void OnEnable()
    {
        GameProgress.OnFlagSet += HandleFlagSet;
        GameProgress.OnReset += Refresh;
        Refresh();
    }

    /// <summary>
    /// 隠れたら登録を解除する（static なイベントなので、必ず解除する）
    /// </summary>
    private void OnDisable()
    {
        GameProgress.OnFlagSet -= HandleFlagSet;
        GameProgress.OnReset -= Refresh;
    }

    /// <summary>
    /// 見ているフラグが立ったときだけ見直す
    /// </summary>
    private void HandleFlagSet(ProgressFlag flag)
    {
        if (flag == _flag)
        {
            Refresh();
        }
    }

    /// <summary>
    /// フラグの状態に合わせて、Target を出す・消す
    /// </summary>
    private void Refresh()
    {
        if (_target == null)
        {
            return;
        }

        bool isSet = GameProgress.HasFlag(_flag);
        _target.SetActive(isSet == _showWhenSet);
    }
}

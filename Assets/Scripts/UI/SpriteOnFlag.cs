using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 進行状況のフラグに合わせて、Image の絵を差し替える部品。
/// 例：Memo のアイコン（Flag：MemoUnlocked。ロック中は鍵付きのメモ帳、解除したらふつうのメモ帳）
/// ・ホーム画面が出るたび（OnEnable）と、フラグが立った・新しいゲームになったときに見直す
/// ・フラグは GameProgress に保存されるので、電池が切れて再起動しても状態が残る
/// </summary>
[RequireComponent(typeof(Image))]
public class SpriteOnFlag : MonoBehaviour
{
    [Header("見るフラグ")]
    [SerializeField] private ProgressFlag _flag;

    [Header("フラグが立つ前の絵（例：鍵付き）")]
    [SerializeField] private Sprite _beforeSprite;

    [Header("フラグが立った後の絵（例：ふつう）")]
    [SerializeField] private Sprite _afterSprite;

    private Image _image;

    /// <summary>
    /// 自分の Image を取得する
    /// </summary>
    private void Awake()
    {
        _image = GetComponent<Image>();
    }

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
    /// フラグの状態に合わせて絵を入れる（絵が空のほうは、今の絵のままにする）
    /// </summary>
    private void Refresh()
    {
        Sprite sprite = GameProgress.HasFlag(_flag) ? _afterSprite : _beforeSprite;

        if (sprite != null)
        {
            _image.sprite = sprite;
        }
    }
}

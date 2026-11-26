using DG.Tweening;
using UnityEngine;

/// <summary>
/// 電池切れの画面（仕様書 v0.2 の3-1）。
/// ・「もう一度起動する」：やり直し。進行状況はそのままで、ホーム画面から再開する（再起動の回数が1増える）
/// ・「← 最初からやり直す」：確認を出し、「やり直す」なら Title へ戻る（次に InGame に入ったときに進行状況が消える）
/// 画面の表示・非表示は PhoneNavigator が行う
/// </summary>
[RequireComponent(typeof(CanvasGroup))]
public class PowerOffScreen : MonoBehaviour
{
    [Header("参照")]
    [SerializeField] private GameManager _gameManager;

    [Header("「最初からやり直しますか？」の確認")]
    [SerializeField] private GameObject _confirmDialog;

    [Header("出てくるときのフェードの秒数")]
    [SerializeField] private float _fadeDuration = 0.8f;

    private CanvasGroup _canvasGroup;

    /// <summary>
    /// 自分の CanvasGroup を覚える
    /// </summary>
    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    /// <summary>
    /// 出てくるたびに確認を閉じて、ふわっと表示する
    /// </summary>
    private void OnEnable()
    {
        _confirmDialog.SetActive(false);

        _canvasGroup.DOKill();
        _canvasGroup.alpha = 0f;
        _canvasGroup.DOFade(1f, _fadeDuration).SetLink(gameObject);
    }

    /// <summary>
    /// 「もう一度起動する」
    /// </summary>
    public void Reboot()
    {
        _gameManager.Boot();
    }

    /// <summary>
    /// 「← 最初からやり直す」：確認を出す
    /// </summary>
    public void OpenConfirm()
    {
        _confirmDialog.SetActive(true);
    }

    /// <summary>
    /// 確認の「キャンセル」
    /// </summary>
    public void CloseConfirm()
    {
        _confirmDialog.SetActive(false);
    }

    /// <summary>
    /// 確認の「やり直す」：Title へ戻る
    /// </summary>
    public void Restart()
    {
        _confirmDialog.SetActive(false);
        _gameManager.ReturnToTitle();
    }
}

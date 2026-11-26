using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/// <summary>
/// スマホ本体のホームボタン（仕様書 v0.2 の4-2）。押すと、どのアプリからでもホーム画面に戻る。
/// キーボードの H か Home キーでも押せる。押したときに少し縮む
/// </summary>
[RequireComponent(typeof(Button))]
public class HomeButton : MonoBehaviour
{
    private const float PressedScale = 0.93f;
    private const float PressDuration = 0.07f;

    [Header("参照")]
    [SerializeField] private PhoneNavigator _navigator;

    private Button _button;

    /// <summary>
    /// 自分のボタンを取得する
    /// </summary>
    private void Awake()
    {
        _button = GetComponent<Button>();
    }

    /// <summary>
    /// 押されたときの処理を登録する
    /// </summary>
    private void Start()
    {
        _button.onClick.AddListener(Press);
    }

    /// <summary>
    /// 登録を解除する
    /// </summary>
    private void OnDestroy()
    {
        _button.onClick.RemoveListener(Press);
    }

    /// <summary>
    /// キーボードの H / Home でも押せるようにする
    /// </summary>
    private void Update()
    {
        if (Keyboard.current == null)
        {
            return;
        }

        if (Keyboard.current.hKey.wasPressedThisFrame || Keyboard.current.homeKey.wasPressedThisFrame)
        {
            Press();
        }
    }

    /// <summary>
    /// 少し縮む演出をして、ホーム画面に戻る（アプリを開いていないときや電源が切れているときは、PhoneNavigator 側で何もしない）
    /// </summary>
    private void Press()
    {
        transform.DOKill();
        transform.localScale = Vector3.one;
        transform.DOScale(PressedScale, PressDuration).SetLoops(2, LoopType.Yoyo).SetLink(gameObject);

        _navigator.ReturnHome();
    }
}

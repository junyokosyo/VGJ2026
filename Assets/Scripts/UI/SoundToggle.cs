using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

/// <summary>
/// 音のオン／オフのボタン（スマホの外、右上の丸いボタン）。
/// タイトル・電池切れ・記録の画面に置く。オフのときは薄く表示する
/// </summary>
[RequireComponent(typeof(Button))]
public class SoundToggle : MonoBehaviour
{
    private const float OffAlpha = 0.6f;

    [Header("参照")]
    [SerializeField] private AudioMixer _audioMixer;
    [SerializeField] private Image _icon;
    [SerializeField] private CanvasGroup _canvasGroup;

    [Header("アイコン（なければ色の濃さだけで表す）")]
    [SerializeField] private Sprite _onSprite;
    [SerializeField] private Sprite _offSprite;

    private Button _button;

    /// <summary>
    /// 自分のボタンを取得する
    /// </summary>
    private void Awake()
    {
        _button = GetComponent<Button>();
    }

    /// <summary>
    /// ボタンと切り替えの通知を登録し、保存されている状態を反映する
    /// </summary>
    private void Start()
    {
        _button.onClick.AddListener(Toggle);
        SoundSettings.OnChanged += Refresh;
        SoundSettings.Apply(_audioMixer);
        Refresh(SoundSettings.IsOn);
    }

    /// <summary>
    /// 登録を解除する
    /// </summary>
    private void OnDestroy()
    {
        _button.onClick.RemoveListener(Toggle);
        SoundSettings.OnChanged -= Refresh;
    }

    /// <summary>
    /// オン／オフを切り替える
    /// </summary>
    private void Toggle()
    {
        SoundSettings.SetOn(!SoundSettings.IsOn, _audioMixer);
    }

    /// <summary>
    /// 見た目をオン／オフに合わせる
    /// </summary>
    private void Refresh(bool isOn)
    {
        if (_icon != null && _onSprite != null && _offSprite != null)
        {
            _icon.sprite = isOn ? _onSprite : _offSprite;
        }

        if (_canvasGroup != null)
        {
            _canvasGroup.alpha = isOn ? 1f : OffAlpha;
        }
    }
}

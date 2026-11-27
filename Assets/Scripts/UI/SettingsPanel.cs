using DG.Tweening;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;

/// <summary>
/// 設定画面（デモ v0.27：右上の歯車から開く）。BGM と SE の音量を変える。
/// 変えた音量はその端末に保存し、AudioMixer（猪瀬さんの Mixer の BGM / SE）にすぐ反映する。
/// BGM のバーを動かすと試しに BGM が鳴り（閉じると静かに止まる）、SE のバーから指を離すと SE が1回鳴る（デモと同じ）。
/// 開く：Open（歯車ボタンの On Click () につなぐ）／閉じる：Close（×ボタン・Esc キー）。
/// 閉じているあいだも、保存した音量をシーンの開始時に反映するため、オブジェクトは表示したまま透明にして押せなくする。
/// プレハブ（Assets/prefab/SettingsPanel）なので、タイトル・電池切れ・記録の画面のどこにでも置ける
/// </summary>
[RequireComponent(typeof(CanvasGroup))]
public class SettingsPanel : MonoBehaviour
{
    [Header("参照")]
    [SerializeField] private AudioMixer _mixer;
    [SerializeField] private VolumeSlider _bgmSlider;
    [SerializeField] private VolumeSlider _seSlider;

    [Header("開くときのフェードの秒数")]
    [SerializeField] private float _fadeDuration = 0.25f;

    private CanvasGroup _canvasGroup;
    private bool _isOpen;

    /// <summary>
    /// 自分の部品を覚えて、バーを動かしたときの処理を登録する
    /// </summary>
    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _bgmSlider.OnChanged += HandleBgmChanged;
        _seSlider.OnChanged += HandleSeChanged;
        _seSlider.OnReleased += HandleSeReleased;
        SetVisible(false);
    }

    /// <summary>
    /// 保存されている音量を Mixer に反映する（Mixer の値は Awake では反映されないことがあるので Start で）
    /// </summary>
    private void Start()
    {
        SoundSettings.Apply(_mixer);
    }

    /// <summary>
    /// 登録を解除する
    /// </summary>
    private void OnDestroy()
    {
        _bgmSlider.OnChanged -= HandleBgmChanged;
        _seSlider.OnChanged -= HandleSeChanged;
        _seSlider.OnReleased -= HandleSeReleased;
    }

    /// <summary>
    /// Esc キーで閉じる
    /// </summary>
    private void Update()
    {
        if (_isOpen && Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Close();
        }
    }

    /// <summary>
    /// 設定画面を開く（歯車ボタンから呼ぶ）
    /// </summary>
    public void Open()
    {
        _bgmSlider.SetValue(SoundSettings.BgmVolume);
        _seSlider.SetValue(SoundSettings.SeVolume);

        SetVisible(true);
        _canvasGroup.alpha = 0f;
        _canvasGroup.DOFade(1f, _fadeDuration).SetLink(gameObject);
    }

    /// <summary>
    /// 設定画面を閉じる（×ボタン・Esc キー）
    /// </summary>
    public void Close()
    {
        if (_isOpen)
        {
            SoundPlayer.EndPreview();
        }

        SetVisible(false);
    }

    /// <summary>
    /// 見える・押せる状態を切り替える（閉じているときは透明で、後ろの画面の操作をじゃましない）
    /// </summary>
    private void SetVisible(bool isVisible)
    {
        _isOpen = isVisible;
        _canvasGroup.DOKill();
        _canvasGroup.alpha = isVisible ? 1f : 0f;
        _canvasGroup.blocksRaycasts = isVisible;
        _canvasGroup.interactable = isVisible;
    }

    /// <summary>
    /// BGM のバーを動かしたとき
    /// </summary>
    private void HandleBgmChanged(float volume)
    {
        SoundSettings.SetBgmVolume(volume, _mixer);
        SoundPlayer.PreviewBgm(volume);
    }

    /// <summary>
    /// SE のバーを動かしたとき
    /// </summary>
    private void HandleSeChanged(float volume)
    {
        SoundSettings.SetSeVolume(volume, _mixer);
    }

    /// <summary>
    /// SE のバーから指を離したとき：今の大きさで1回鳴らす
    /// </summary>
    private void HandleSeReleased()
    {
        SoundPlayer.PlaySe(SeKind.Tap);
    }
}

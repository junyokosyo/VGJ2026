using System;
using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// 音の設定。選んだ状態は PlayerPrefs に保存し、次に開いたときも引き継ぐ。
/// ・BGM と SE の音量（デモ v0.27 の設定画面）：0〜1 を 5% 刻み。AudioMixer の BGM / SE に反映する。
///   つまみの位置を耳の感じに合わせるため、2乗してから dB にする（デモと同じ）
/// ・音のオン／オフ（仕様書 v0.2 の8章）：AudioMixer の Master を 0dB（オン）／-80dB（オフ）に切り替える
/// </summary>
public static class SoundSettings
{
    private const string SaveKey = "nosubject-sound";
    private const string BgmVolumeKey = "nosubject-vol-bgm";
    private const string SeVolumeKey = "nosubject-vol-se";
    private const string MasterParameter = "Master";
    private const string BgmParameter = "BGM";
    private const string SeParameter = "SE";
    private const float VolumeStep = 0.05f;
    private const float OnVolume = 0f;
    private const float OffVolume = -80f;

    /// <summary>
    /// オン／オフが切り替わったとき（引数：オンなら true）。
    /// static なイベントなので、登録した側は OnDestroy で必ず解除すること
    /// </summary>
    public static event Action<bool> OnChanged;

    /// <summary>
    /// 音がオンか（はじめて遊ぶときはオン）
    /// </summary>
    public static bool IsOn => PlayerPrefs.GetInt(SaveKey, 1) == 1;

    /// <summary>
    /// BGM の音量（0〜1。はじめて遊ぶときは 1）
    /// </summary>
    public static float BgmVolume => PlayerPrefs.GetFloat(BgmVolumeKey, 1f);

    /// <summary>
    /// SE の音量（0〜1。はじめて遊ぶときは 1）
    /// </summary>
    public static float SeVolume => PlayerPrefs.GetFloat(SeVolumeKey, 1f);

    /// <summary>
    /// BGM の音量を 5% 刻みにして保存し、Mixer に反映する
    /// </summary>
    public static void SetBgmVolume(float volume, AudioMixer mixer)
    {
        PlayerPrefs.SetFloat(BgmVolumeKey, RoundVolume(volume));
        PlayerPrefs.Save();
        Apply(mixer);
    }

    /// <summary>
    /// SE の音量を 5% 刻みにして保存し、Mixer に反映する
    /// </summary>
    public static void SetSeVolume(float volume, AudioMixer mixer)
    {
        PlayerPrefs.SetFloat(SeVolumeKey, RoundVolume(volume));
        PlayerPrefs.Save();
        Apply(mixer);
    }

    /// <summary>
    /// 0〜1 に収めて、5% 刻みにする
    /// </summary>
    public static float RoundVolume(float volume)
    {
        return Mathf.Round(Mathf.Clamp01(volume) / VolumeStep) * VolumeStep;
    }

    /// <summary>
    /// オン／オフを切り替えて保存し、Mixer に反映する
    /// </summary>
    public static void SetOn(bool isOn, AudioMixer mixer)
    {
        PlayerPrefs.SetInt(SaveKey, isOn ? 1 : 0);
        PlayerPrefs.Save();
        Apply(mixer);
        OnChanged?.Invoke(isOn);
    }

    /// <summary>
    /// 保存されているオン／オフを Mixer に反映する（Mixer の値は Awake では反映されないことがあるので、Start 以降に呼ぶ）
    /// </summary>
    public static void Apply(AudioMixer mixer)
    {
        if (mixer == null)
        {
            return;
        }

        mixer.SetFloat(MasterParameter, IsOn ? OnVolume : OffVolume);
        mixer.SetFloat(BgmParameter, ToDecibel(BgmVolume));
        mixer.SetFloat(SeParameter, ToDecibel(SeVolume));
    }

    /// <summary>
    /// 音量（0〜1）を dB にする。2乗してから変えるので、つまみの真ん中で「半分くらい」に聞こえる。0 なら -80dB（無音）
    /// </summary>
    private static float ToDecibel(float volume)
    {
        float level = volume * volume;
        return level <= 0.0001f ? OffVolume : Mathf.Max(OffVolume, 20f * Mathf.Log10(level));
    }
}

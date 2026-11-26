using System;
using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// 音のオン／オフ（仕様書 v0.2 の8章）。選んだ状態は PlayerPrefs に保存し、次に開いたときも引き継ぐ。
/// AudioMixer の Master を 0dB（オン）／-80dB（オフ）に切り替える
/// </summary>
public static class SoundSettings
{
    private const string SaveKey = "nosubject-sound";
    private const string MasterParameter = "Master";
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
    }
}

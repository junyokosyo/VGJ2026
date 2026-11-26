using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;


public class AudioSetting : MonoBehaviour
{
    //AudioMixerを入れる。
    [SerializeField] private AudioMixer audioMixer;

    //スライダーを入れる。
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider seSlider;

    public void SetMaster(float volume)
    {
        audioMixer.SetFloat("Master", volume);
    }

    public void SetBGM(float volume)
    {
        audioMixer.SetFloat("BGM", volume);
    }
    public void SetSE(float volume)
    {
        audioMixer.SetFloat("SE", volume);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //ミキサーのvolumeにスライダーのvolumeを入れてます。

        // Master
        audioMixer.GetFloat("Master", out float masterVolume);
        masterSlider.value = masterVolume;
        //BGM
        audioMixer.GetFloat("BGM", out float bgmVolume);
        bgmSlider.value = bgmVolume;
        //SE
        audioMixer.GetFloat("SE", out float seVolume);
        seSlider.value = seVolume;
    }

    // Update is called once per frame
    void Update()
    {

    }
}

using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    public AudioMixerGroup masterGroup;
    public Slider audioSlider;

    public void Start()
    {
        audioSlider.value = PlayerPrefs.GetFloat("Volume", 1f);

        SetMasterVolume(audioSlider.value);

        audioSlider.onValueChanged.AddListener(VolumeChanged);
    }

    private void VolumeChanged(float value)
    {
        SetMasterVolume(value);
    }

    public void SetMasterVolume(float vol)
    {
        PlayerPrefs.SetFloat("Volume", vol);

        vol = Mathf.Clamp(vol, 0.0001f, 1f);
        masterGroup.audioMixer.SetFloat("MasterVolume", Mathf.Log10(vol) * 20);
    }
}

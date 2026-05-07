using UnityEngine;
using UnityEngine.UI;

public class AudioSettingsUI : MonoBehaviour
{
    public Slider musicSlider;
    public Slider sfxSlider;
    public Toggle musicToggle;
    public Toggle sfxToggle;

    private void Start()
    {
        if (AudioManager.Instance == null)
        {
            Debug.LogWarning("AudioManager not found in scene.");
            return;
        }

        if (musicSlider != null)
        {
            musicSlider.value = AudioManager.Instance.GetMusicVolume();
            musicSlider.onValueChanged.AddListener(AudioManager.Instance.SetMusicVolume);
        }

        if (sfxSlider != null)
        {
            sfxSlider.value = AudioManager.Instance.GetSFXVolume();
            sfxSlider.onValueChanged.AddListener(AudioManager.Instance.SetSFXVolume);
        }

        if (musicToggle != null)
        {
            musicToggle.isOn = !AudioManager.Instance.IsMusicMuted();
            musicToggle.onValueChanged.AddListener((isOn) => AudioManager.Instance.SetMusicMute(!isOn));
        }

        if (sfxToggle != null)
        {
            sfxToggle.isOn = !AudioManager.Instance.IsSFXMuted();
            sfxToggle.onValueChanged.AddListener((isOn) => AudioManager.Instance.SetSFXMute(!isOn));
        }
    }

    private void OnDestroy()
    {
        if (musicSlider != null) musicSlider.onValueChanged.RemoveAllListeners();
        if (sfxSlider != null) sfxSlider.onValueChanged.RemoveAllListeners();
        if (musicToggle != null) musicToggle.onValueChanged.RemoveAllListeners();
        if (sfxToggle != null) sfxToggle.onValueChanged.RemoveAllListeners();
    }
}

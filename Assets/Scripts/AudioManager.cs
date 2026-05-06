using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Default Volumes")]
    [Range(0f, 1f)] public float defaultMusicVolume = 1f;
    [Range(0f, 1f)] public float defaultSFXVolume = 1f;

    private const string PrefMusicVolume = "AudioManager_MusicVolume";
    private const string PrefSFXVolume = "AudioManager_SFXVolume";
    private const string PrefMusicMuted = "AudioManager_MusicMuted";
    private const string PrefSFXMuted = "AudioManager_SFXMuted";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.loop = true;
            musicSource.playOnAwake = false;
        }

        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.loop = false;
            sfxSource.playOnAwake = false;
        }

        float musicVol = PlayerPrefs.HasKey(PrefMusicVolume) ? PlayerPrefs.GetFloat(PrefMusicVolume) : defaultMusicVolume;
        float sfxVol = PlayerPrefs.HasKey(PrefSFXVolume) ? PlayerPrefs.GetFloat(PrefSFXVolume) : defaultSFXVolume;
        bool musicMuted = PlayerPrefs.GetInt(PrefMusicMuted, 0) == 1;
        bool sfxMuted = PlayerPrefs.GetInt(PrefSFXMuted, 0) == 1;

        SetMusicVolume(musicVol);
        SetSFXVolume(sfxVol);
        SetMusicMute(musicMuted);
        SetSFXMute(sfxMuted);
    }

    private void EnsureSources()
    {
        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.loop = true;
            musicSource.playOnAwake = false;
        }

        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.loop = false;
            sfxSource.playOnAwake = false;
        }
    }

    public void PlayMusic(AudioClip clip, float volumeScale = 1f, bool loop = true)
    {
        if (clip == null)
        {
            Debug.LogWarning("AudioManager.PlayMusic called with null clip");
            return;
        }

        EnsureSources();

        // If same clip and already playing, restart to ensure it's audible when returning to menu
        bool sameClip = musicSource.clip == clip;
        musicSource.clip = clip;
        musicSource.loop = loop;
        musicSource.volume = defaultMusicVolume * Mathf.Clamp01(volumeScale);
        if (sameClip)
        {
            musicSource.Stop();
        }
        musicSource.Play();
        Debug.Log($"AudioManager: PlayMusic '{clip.name}' loop={loop} vol={musicSource.volume}");
    }

    public void StopMusic()
    {
        musicSource.Stop();
        musicSource.clip = null;
    }

    public void PlaySFX(AudioClip clip, float volumeScale = 1f)
    {
        if (clip == null)
        {
            Debug.LogWarning("AudioManager.PlaySFX called with null clip");
            return;
        }

        EnsureSources();

        if (sfxSource == null)
        {
            Debug.LogWarning("AudioManager: sfxSource is null, cannot play SFX");
            return;
        }

        sfxSource.PlayOneShot(clip, sfxSource.volume * Mathf.Clamp01(volumeScale));
        Debug.Log($"AudioManager: PlaySFX '{clip.name}' vol={sfxSource.volume}");
    }

    public void SetMusicVolume(float vol)
    {
        defaultMusicVolume = Mathf.Clamp01(vol);
        if (musicSource != null) musicSource.volume = defaultMusicVolume;
        PlayerPrefs.SetFloat(PrefMusicVolume, defaultMusicVolume);
        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float vol)
    {
        defaultSFXVolume = Mathf.Clamp01(vol);
        if (sfxSource != null) sfxSource.volume = defaultSFXVolume;
        PlayerPrefs.SetFloat(PrefSFXVolume, defaultSFXVolume);
        PlayerPrefs.Save();
    }

    public void SetMusicMute(bool mute)
    {
        if (musicSource != null) musicSource.mute = mute;
        PlayerPrefs.SetInt(PrefMusicMuted, mute ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void SetSFXMute(bool mute)
    {
        if (sfxSource != null) sfxSource.mute = mute;
        PlayerPrefs.SetInt(PrefSFXMuted, mute ? 1 : 0);
        PlayerPrefs.Save();
    }

    public float GetMusicVolume() => defaultMusicVolume;
    public float GetSFXVolume() => defaultSFXVolume;
    public bool IsMusicMuted() => musicSource != null && musicSource.mute;
    public bool IsSFXMuted() => sfxSource != null && sfxSource.mute;
}

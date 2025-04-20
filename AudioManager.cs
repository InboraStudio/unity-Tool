using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;
using System;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource ambienceSource;
    [SerializeField] private AudioSource uiSource;
    
    [Header("Audio Mixers")]
    [SerializeField] private AudioMixer audioMixer;
    
    [Header("Sound Collections")]
    [SerializeField] private Sound[] musicTracks;
    [SerializeField] private Sound[] sfxSounds;
    [SerializeField] private Sound[] ambienceSounds;
    [SerializeField] private Sound[] uiSounds;
    
    // Dictionary for quick lookup of sounds by name
    private Dictionary<string, Sound> soundDictionary = new Dictionary<string, Sound>();
    
    // Track currently playing sounds
    private string currentMusicTrack;
    private string currentAmbience;
    
    // Volume settings
    private float masterVolume = 1f;
    private float musicVolume = 1f;
    private float sfxVolume = 1f;
    private float ambienceVolume = 1f;
    private float uiVolume = 1f;
    
    // For fade effects
    private Coroutine musicFadeCoroutine;
    private Coroutine ambienceFadeCoroutine;
    
    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        // Initialize sound dictionary
        RegisterSounds(musicTracks);
        RegisterSounds(sfxSounds);
        RegisterSounds(ambienceSounds);
        RegisterSounds(uiSounds);
        
        // Set up audio sources if not assigned
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
        
        if (ambienceSource == null)
        {
            ambienceSource = gameObject.AddComponent<AudioSource>();
            ambienceSource.loop = true;
            ambienceSource.playOnAwake = false;
        }
        
        if (uiSource == null)
        {
            uiSource = gameObject.AddComponent<AudioSource>();
            uiSource.loop = false;
            uiSource.playOnAwake = false;
        }
        
        // Load saved volumes
        LoadVolumeSettings();
        ApplyVolumeSettings();
    }
    
    private void RegisterSounds(Sound[] sounds)
    {
        foreach (Sound sound in sounds)
        {
            if (!soundDictionary.ContainsKey(sound.name))
            {
                soundDictionary.Add(sound.name, sound);
            }
            else
            {
                Debug.LogWarning("Duplicate sound name found: " + sound.name);
            }
        }
    }
    
    #region Music Functions
    
    public void PlayMusic(string name, float fadeInDuration = 1f)
    {
        if (name == currentMusicTrack && musicSource.isPlaying)
            return;
            
        if (!soundDictionary.ContainsKey(name))
        {
            Debug.LogWarning("Music track not found: " + name);
            return;
        }
        
        Sound sound = soundDictionary[name];
        
        if (fadeInDuration > 0 && musicSource.isPlaying)
        {
            StartCoroutine(FadeMusicTrack(sound, fadeInDuration));
        }
        else
        {
            musicSource.clip = sound.clip;
            musicSource.volume = sound.volume * musicVolume;
            musicSource.pitch = sound.pitch;
            musicSource.Play();
        }
        
        currentMusicTrack = name;
    }
    
    private System.Collections.IEnumerator FadeMusicTrack(Sound newTrack, float fadeDuration)
    {
        float startVolume = musicSource.volume;
        float timer = 0;
        
        // Fade out current track
        while (timer < fadeDuration / 2)
        {
            timer += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(startVolume, 0, timer / (fadeDuration / 2));
            yield return null;
        }
        
        // Switch to new track
        musicSource.clip = newTrack.clip;
        musicSource.pitch = newTrack.pitch;
        musicSource.Play();
        
        // Fade in new track
        timer = 0;
        while (timer < fadeDuration / 2)
        {
            timer += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(0, newTrack.volume * musicVolume, timer / (fadeDuration / 2));
            yield return null;
        }
        
        musicSource.volume = newTrack.volume * musicVolume;
    }
    
    public void StopMusic(float fadeOutDuration = 1f)
    {
        if (!musicSource.isPlaying)
            return;
            
        if (fadeOutDuration > 0)
        {
            StartCoroutine(FadeOut(musicSource, fadeOutDuration));
        }
        else
        {
            musicSource.Stop();
        }
        
        currentMusicTrack = null;
    }
    
    private System.Collections.IEnumerator FadeOut(AudioSource source, float fadeDuration)
    {
        float startVolume = source.volume;
        float timer = 0;
        
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            source.volume = Mathf.Lerp(startVolume, 0, timer / fadeDuration);
            yield return null;
        }
        
        source.Stop();
        source.volume = startVolume;
    }
    
    #endregion
    
    #region SFX Functions
    
    public void PlaySFX(string name)
    {
        if (!soundDictionary.ContainsKey(name))
        {
            Debug.LogWarning("SFX not found: " + name);
            return;
        }
        
        Sound sound = soundDictionary[name];
        sfxSource.PlayOneShot(sound.clip, sound.volume * sfxVolume);
    }
    
    public void PlaySFXAtPosition(string name, Vector3 position, float minDistance = 1f, float maxDistance = 50f)
    {
        if (!soundDictionary.ContainsKey(name))
        {
            Debug.LogWarning("SFX not found: " + name);
            return;
        }
        
        Sound sound = soundDictionary[name];
        
        // Create temporary audio source at the specified position
        GameObject tempAudio = new GameObject("TempAudio");
        tempAudio.transform.position = position;
        
        AudioSource tempSource = tempAudio.AddComponent<AudioSource>();
        tempSource.clip = sound.clip;
        tempSource.volume = sound.volume * sfxVolume;
        tempSource.pitch = sound.pitch;
        tempSource.spatialBlend = 1f; // 3D sound
        tempSource.minDistance = minDistance;
        tempSource.maxDistance = maxDistance;
        tempSource.rolloffMode = AudioRolloffMode.Linear;
        tempSource.Play();
        
        // Destroy the GameObject after the sound is done playing
        float clipLength = sound.clip.length;
        Destroy(tempAudio, clipLength + 0.1f);
    }
    
    #endregion
    
    #region Ambience Functions
    
    public void PlayAmbience(string name, float fadeInDuration = 2f)
    {
        if (name == currentAmbience && ambienceSource.isPlaying)
            return;
            
        if (!soundDictionary.ContainsKey(name))
        {
            Debug.LogWarning("Ambience not found: " + name);
            return;
        }
        
        Sound sound = soundDictionary[name];
        
        if (fadeInDuration > 0 && ambienceSource.isPlaying)
        {
            StartCoroutine(FadeAmbienceTrack(sound, fadeInDuration));
        }
        else
        {
            ambienceSource.clip = sound.clip;
            ambienceSource.volume = sound.volume * ambienceVolume;
            ambienceSource.pitch = sound.pitch;
            ambienceSource.Play();
        }
        
        currentAmbience = name;
    }
    
    private System.Collections.IEnumerator FadeAmbienceTrack(Sound newTrack, float fadeDuration)
    {
        float startVolume = ambienceSource.volume;
        float timer = 0;
        
        // Fade out current track
        while (timer < fadeDuration / 2)
        {
            timer += Time.deltaTime;
            ambienceSource.volume = Mathf.Lerp(startVolume, 0, timer / (fadeDuration / 2));
            yield return null;
        }
        
        // Switch to new track
        ambienceSource.clip = newTrack.clip;
        ambienceSource.pitch = newTrack.pitch;
        ambienceSource.Play();
        
        // Fade in new track
        timer = 0;
        while (timer < fadeDuration / 2)
        {
            timer += Time.deltaTime;
            ambienceSource.volume = Mathf.Lerp(0, newTrack.volume * ambienceVolume, timer / (fadeDuration / 2));
            yield return null;
        }
        
        ambienceSource.volume = newTrack.volume * ambienceVolume;
    }
    
    public void StopAmbience(float fadeOutDuration = 2f)
    {
        if (!ambienceSource.isPlaying)
            return;
            
        if (fadeOutDuration > 0)
        {
            StartCoroutine(FadeOut(ambienceSource, fadeOutDuration));
        }
        else
        {
            ambienceSource.Stop();
        }
        
        currentAmbience = null;
    }
    
    #endregion
    
    #region UI Sound Functions
    
    public void PlayUISound(string name)
    {
        if (!soundDictionary.ContainsKey(name))
        {
            Debug.LogWarning("UI sound not found: " + name);
            return;
        }
        
        Sound sound = soundDictionary[name];
        uiSource.PlayOneShot(sound.clip, sound.volume * uiVolume);
    }
    
    #endregion
    
    #region Volume Control
    
    public void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);
        ApplyVolumeSettings();
        SaveVolumeSettings();
    }
    
    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        ApplyVolumeSettings();
        SaveVolumeSettings();
    }
    
    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        ApplyVolumeSettings();
        SaveVolumeSettings();
    }
    
    public void SetAmbienceVolume(float volume)
    {
        ambienceVolume = Mathf.Clamp01(volume);
        ApplyVolumeSettings();
        SaveVolumeSettings();
    }
    
    public void SetUIVolume(float volume)
    {
        uiVolume = Mathf.Clamp01(volume);
        ApplyVolumeSettings();
        SaveVolumeSettings();
    }
    
    public float GetMasterVolume() => masterVolume;
    public float GetMusicVolume() => musicVolume;
    public float GetSFXVolume() => sfxVolume;
    public float GetAmbienceVolume() => ambienceVolume;
    public float GetUIVolume() => uiVolume;
    
    private void ApplyVolumeSettings()
    {
        // Apply volumes to audio mixer if available
        if (audioMixer != null)
        {
            // Convert linear volume to logarithmic for better control
            float masterVolumeDB = masterVolume > 0.001f ? 20f * Mathf.Log10(masterVolume) : -80f;
            float musicVolumeDB = musicVolume > 0.001f ? 20f * Mathf.Log10(musicVolume) : -80f;
            float sfxVolumeDB = sfxVolume > 0.001f ? 20f * Mathf.Log10(sfxVolume) : -80f;
            float ambienceVolumeDB = ambienceVolume > 0.001f ? 20f * Mathf.Log10(ambienceVolume) : -80f;
            float uiVolumeDB = uiVolume > 0.001f ? 20f * Mathf.Log10(uiVolume) : -80f;
            
            audioMixer.SetFloat("MasterVolume", masterVolumeDB);
            audioMixer.SetFloat("MusicVolume", musicVolumeDB);
            audioMixer.SetFloat("SFXVolume", sfxVolumeDB);
            audioMixer.SetFloat("AmbienceVolume", ambienceVolumeDB);
            audioMixer.SetFloat("UIVolume", uiVolumeDB);
        }
        
        // Apply directly to sources if mixer not available
        if (musicSource != null && musicSource.clip != null && soundDictionary.ContainsKey(currentMusicTrack))
        {
            musicSource.volume = soundDictionary[currentMusicTrack].volume * musicVolume * masterVolume;
        }
        
        if (ambienceSource != null && ambienceSource.clip != null && soundDictionary.ContainsKey(currentAmbience))
        {
            ambienceSource.volume = soundDictionary[currentAmbience].volume * ambienceVolume * masterVolume;
        }
    }
    
    private void SaveVolumeSettings()
    {
        PlayerPrefs.SetFloat("MasterVolume", masterVolume);
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
        PlayerPrefs.SetFloat("AmbienceVolume", ambienceVolume);
        PlayerPrefs.SetFloat("UIVolume", uiVolume);
        PlayerPrefs.Save();
    }
    
    private void LoadVolumeSettings()
    {
        masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        ambienceVolume = PlayerPrefs.GetFloat("AmbienceVolume", 1f);
        uiVolume = PlayerPrefs.GetFloat("UIVolume", 1f);
    }
    
    #endregion
}

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
    [Range(0f, 1f)]
    public float volume = 1f;
    [Range(0.1f, 3f)]
    public float pitch = 1f;
} 
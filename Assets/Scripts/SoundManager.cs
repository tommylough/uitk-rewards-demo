using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class SoundClip
{
    public string name;
    public AudioClip clip;
    [Range(0f, 1f)]
    public float volume = 1f;
    [Range(0.1f, 3f)]
    public float pitch = 1f;
    public bool loop = false;
}

[System.Serializable]
public class SFXClip
{
    public string name;
    public AudioClip clip;
    [Range(0f, 1f)]
    public float volume = 1f;
    [Range(0.1f, 3f)]
    public float pitch = 1f;
}

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;
    
    [Header("Audio Sources")]
    public AudioSource sfxAudioSource;
    public AudioSource loopingSFXAudioSource;
    public AudioSource musicAudioSource;
    
    [Header("Sound Effects")]
    public List<SFXClip> soundEffects = new List<SFXClip>();
    
    [Header("Background Music")]
    public List<SoundClip> backgroundMusic = new List<SoundClip>();
    
    [Header("Volume Settings")]
    [Range(0f, 1f)]
    public float masterVolume = 1f;
    [Range(0f, 1f)]
    public float sfxVolume = 1f;
    [Range(0f, 1f)]
    public float musicVolume = 1f;
    
    Dictionary<string, SFXClip> sfxDict = new Dictionary<string, SFXClip>();
    Dictionary<string, SoundClip> musicDict = new Dictionary<string, SoundClip>();
    
    void Awake()
    {
        // Register this instance for global access
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Initialize();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Initialize()
    {
        // Create audio sources if not assigned
        if (sfxAudioSource == null)
        {
            sfxAudioSource = gameObject.AddComponent<AudioSource>();
            sfxAudioSource.playOnAwake = false;
        }
        
        if (loopingSFXAudioSource == null)
        {
            loopingSFXAudioSource = gameObject.AddComponent<AudioSource>();
            loopingSFXAudioSource.playOnAwake = false;
            loopingSFXAudioSource.loop = true;
        }
        
        if (musicAudioSource == null)
        {
            musicAudioSource = gameObject.AddComponent<AudioSource>();
            musicAudioSource.playOnAwake = false;
            musicAudioSource.loop = true;
        }
        
        // Populate dictionaries for quick lookup
        PopulateDictionaries();
        
        // Set initial volumes
        UpdateVolumes();
    }
    
    void PopulateDictionaries()
    {
        sfxDict.Clear();
        musicDict.Clear();
        
        foreach (SFXClip sound in soundEffects)
        {
            if (!string.IsNullOrEmpty(sound.name) && sound.clip != null)
            {
                sfxDict[sound.name] = sound;
            }
        }
        
        foreach (SoundClip music in backgroundMusic)
        {
            if (!string.IsNullOrEmpty(music.name) && music.clip != null)
            {
                musicDict[music.name] = music;
            }
        }
    }
    
    void UpdateVolumes()
    {
        sfxAudioSource.volume = masterVolume * sfxVolume;
        loopingSFXAudioSource.volume = masterVolume * sfxVolume;
        musicAudioSource.volume = masterVolume * musicVolume;
    }
    
    // Sound Effects Methods
    public void PlaySFX(string soundName)
    {
        if (sfxDict.ContainsKey(soundName))
        {
            SFXClip sound = sfxDict[soundName];
            sfxAudioSource.pitch = sound.pitch;
            sfxAudioSource.PlayOneShot(sound.clip, sound.volume);
        }
        else
        {
            Debug.LogWarning($"Sound effect '{soundName}' not found!");
        }
    }
    
    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        if (clip != null)
        {
            sfxAudioSource.PlayOneShot(clip, volume);
        }
    }
    
    // Looping Sound Effects Methods
    public void PlayLoopingSFX(string soundName, float duration = 0f)
    {
        if (sfxDict.ContainsKey(soundName))
        {
            SFXClip sound = sfxDict[soundName];
            PlayLoopingSFX(sound.clip, sound.volume, sound.pitch, duration);
        }
        else
        {
            Debug.LogWarning($"Sound effect '{soundName}' not found!");
        }
    }
    
    public void PlayLoopingSFX(AudioClip clip, float volume = 1f, float pitch = 1f, float duration = 0f)
    {
        if (clip != null)
        {
            loopingSFXAudioSource.clip = clip;
            loopingSFXAudioSource.volume = volume * masterVolume * sfxVolume;
            loopingSFXAudioSource.pitch = pitch;
            loopingSFXAudioSource.Play();
            
            if (duration > 0f)
            {
                StartCoroutine(StopLoopingSFXAfterDelay(duration));
            }
        }
    }
    
    public void StopLoopingSFX(bool fadeOut = false, float fadeDuration = 0.5f)
    {
        if (fadeOut)
        {
            StartCoroutine(FadeOutLoopingSFX(fadeDuration));
        }
        else
        {
            loopingSFXAudioSource.Stop();
        }
    }
    
    public bool IsLoopingSFXPlaying()
    {
        return loopingSFXAudioSource.isPlaying;
    }
    
    // Background Music Methods
    public void PlayMusic(string musicName, bool fadeIn = false, float fadeDuration = 1f)
    {
        if (musicDict.ContainsKey(musicName))
        {
            SoundClip music = musicDict[musicName];
            musicAudioSource.clip = music.clip;
            musicAudioSource.loop = music.loop;
            musicAudioSource.pitch = music.pitch;
            
            if (fadeIn)
            {
                StartCoroutine(FadeIn(fadeDuration));
            }
            else
            {
                musicAudioSource.Play();
            }
        }
        else
        {
            Debug.LogWarning($"Background music '{musicName}' not found!");
        }
    }
    
    public void StopMusic(bool fadeOut = false, float fadeDuration = 1f)
    {
        if (fadeOut)
        {
            StartCoroutine(FadeOut(fadeDuration));
        }
        else
        {
            musicAudioSource.Stop();
        }
    }
    
    public void PauseMusic()
    {
        musicAudioSource.Pause();
    }
    
    public void ResumeMusic()
    {
        musicAudioSource.UnPause();
    }
    
    // Volume Control Methods
    public void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);
        UpdateVolumes();
    }
    
    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        UpdateVolumes();
    }
    
    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        UpdateVolumes();
    }
    
    // Fade Effects
    System.Collections.IEnumerator FadeIn(float duration)
    {
        musicAudioSource.volume = 0f;
        musicAudioSource.Play();
        
        float targetVolume = masterVolume * musicVolume;
        float currentTime = 0f;
        
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            musicAudioSource.volume = Mathf.Lerp(0f, targetVolume, currentTime / duration);
            yield return null;
        }
        
        musicAudioSource.volume = targetVolume;
    }
    
    System.Collections.IEnumerator FadeOut(float duration)
    {
        float startVolume = musicAudioSource.volume;
        float currentTime = 0f;
        
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            musicAudioSource.volume = Mathf.Lerp(startVolume, 0f, currentTime / duration);
            yield return null;
        }
        
        musicAudioSource.Stop();
        musicAudioSource.volume = startVolume;
    }
    
    System.Collections.IEnumerator StopLoopingSFXAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        loopingSFXAudioSource.Stop();
    }
    
    System.Collections.IEnumerator FadeOutLoopingSFX(float duration)
    {
        float startVolume = loopingSFXAudioSource.volume;
        float currentTime = 0f;
        
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            loopingSFXAudioSource.volume = Mathf.Lerp(startVolume, 0f, currentTime / duration);
            yield return null;
        }
        
        loopingSFXAudioSource.Stop();
        loopingSFXAudioSource.volume = startVolume;
    }
    
    // Utility Methods
    public bool IsMusicPlaying()
    {
        return musicAudioSource.isPlaying;
    }
    
    public bool IsMusicPaused()
    {
        return !musicAudioSource.isPlaying && musicAudioSource.time > 0;
    }
    
    public void StopAllSounds()
    {
        sfxAudioSource.Stop();
        loopingSFXAudioSource.Stop();
        musicAudioSource.Stop();
    }
}
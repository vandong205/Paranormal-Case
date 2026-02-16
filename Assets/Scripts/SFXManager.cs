using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Threading.Tasks;
public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Global Volume")]
    [Range(0f, 1f)] public float musicVolume = 1f;
    [Range(0f, 1f)] public float sfxVolume = 1f;
    private Dictionary<Music, AudioClip> musicCache = new();
    private Dictionary<Music, AsyncOperationHandle<AudioClip>> handleCache = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    public void PlayMusic(Music music, float overrideFadeDuration = -1f)
{
    if (music == null || music.clip == null)
        return;

    StopAllCoroutines();
    StartCoroutine(PlayMusicRoutine(music, overrideFadeDuration));
}

public async Task PreloadMusic(Music music)
{
    if (music == null || music.clip == null)
        return;

    if (musicCache.ContainsKey(music))
        return;

    var handle = music.clip.LoadAssetAsync<AudioClip>();
    handleCache[music] = handle;

    AudioClip clip = await handle.Task;

    musicCache[music] = clip;
}
private IEnumerator PlayMusicRoutine(Music music, float overrideFadeDuration)
{
    float fadeDuration = overrideFadeDuration >= 0f 
        ? overrideFadeDuration 
        : music.fadeInDuration;

    if (fadeDuration > 0f)
        yield return StartCoroutine(FadeOut(musicSource, fadeDuration));

    if (!musicCache.ContainsKey(music))
    {
        var preloadTask = PreloadMusic(music);
        while (!preloadTask.IsCompleted)
            yield return null;
    }

    musicSource.clip = musicCache[music];
    musicSource.loop = music.loop;
    musicSource.outputAudioMixerGroup = music.outputMixerGroup;
    musicSource.time = music.startTime;
    musicSource.volume = 0f;
    musicSource.spatialBlend = 0f;

    musicSource.Play();

    if (fadeDuration > 0f)
        yield return StartCoroutine(FadeIn(musicSource, music.volume * musicVolume, fadeDuration));
    else
        musicSource.volume = music.volume * musicVolume;
}


    public void StopMusic(float fadeDuration = 0f)
    {
        if (fadeDuration > 0f)
            StartCoroutine(FadeOutAndStop(musicSource, fadeDuration));
        else
            musicSource.Stop();
    }

    // =========================
    // SFX
    // =========================

    public void PlaySFX(SoundEffect sound)
    {
        if (sound == null || sound.clip == null)
            return;

        sfxSource.pitch = sound.GetRandomPitch();
        sfxSource.PlayOneShot(sound.clip, sound.volume * sfxVolume);
    }

    public void PlaySFXAtPosition(SoundEffect sound, Vector3 position)
    {
        if (sound == null || sound.clip == null)
            return;

        GameObject tempObj = new GameObject("SFX_" + sound.clip.name);
        tempObj.transform.position = position;

        AudioSource source = tempObj.AddComponent<AudioSource>();
        source.clip = sound.clip;
        source.volume = sound.volume * sfxVolume;
        source.pitch = sound.GetRandomPitch();
        source.spatialBlend = sound.spatialBlend;
        source.minDistance = sound.minDistance;
        source.maxDistance = sound.maxDistance;
        source.loop = false;

        source.Play();
        Destroy(tempObj, sound.clip.length);
    }

    // =========================
    // Fade Utilities
    // =========================

    private IEnumerator FadeIn(AudioSource source, float targetVolume, float duration)
    {
        float startVolume = 0f;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            source.volume = Mathf.Lerp(startVolume, targetVolume, time / duration);
            yield return null;
        }

        source.volume = targetVolume;
    }

    private IEnumerator FadeOut(AudioSource source, float duration)
    {
        float startVolume = source.volume;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            source.volume = Mathf.Lerp(startVolume, 0f, time / duration);
            yield return null;
        }

        source.volume = 0f;
    }

    private IEnumerator FadeOutAndStop(AudioSource source, float duration)
    {
        yield return StartCoroutine(FadeOut(source, duration));
        source.Stop();
    }
}

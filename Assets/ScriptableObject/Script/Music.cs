
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "Music", menuName = "Scriptable Objects/Music")]
public class Music : ScriptableObject
{
    [Header("Audio Clip")]
    public AssetReferenceT<AudioClip> clip;

    [Header("Volume")]
    [Range(0f, 1f)]
    public float volume = 1f;

    [Header("Playback Settings")]
    public bool loop = true;

    [Tooltip("Fade in duration when music starts")]
    public float fadeInDuration = 1f;

    [Tooltip("Start time in seconds (useful for resume)")]
    public float startTime = 0f;

    [Header("Mixer (Optional)")]
    public AudioMixerGroup outputMixerGroup;
}

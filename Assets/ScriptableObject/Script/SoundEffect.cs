using UnityEngine;

[CreateAssetMenu(fileName = "SoundEffect", menuName = "Scriptable Objects/Effect/SoundEffect")]
public class SoundEffect : ScriptableObject
{
    [Header("Audio Clip")]
    public AudioClip clip;

    [Header("Volume")]
    [Range(0f, 1f)]
    public float volume = 1f;

    [Header("Pitch")]
    public Vector2 pitchRange = new Vector2(1f, 1f);

    [Header("Spatial Settings")]
    [Range(0f, 1f)]
    public float spatialBlend = 0f; // 0 = 2D, 1 = 3D

    public float minDistance = 1f;
    public float maxDistance = 15f;

    [Header("Playback")]
    public bool loop = false;

    public float GetRandomPitch()
    {
        return Random.Range(pitchRange.x, pitchRange.y);
    }
}

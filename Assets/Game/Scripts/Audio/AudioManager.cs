using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    private AudioSource _audiosource;

    private void Awake() => _audiosource = GetComponent<AudioSource>();

    /// <summary>
    /// Plays an audio file at max volume.
    /// </summary>
    /// <param name="clip">The AudioClip to play.</param>
    public void PlaySound(AudioClip clip) => _audiosource?.PlayOneShot(clip, 1f);

    /// <summary>
    /// Plays an audio file with a set volume.
    /// </summary>
    /// <param name="clip">The AudioClip to play.</param>
    /// <param name="volume">The volume of the AudioClip to play.</param>
    public void PlaySoundWithVolume(AudioClip clip, float volume) => _audiosource?.PlayOneShot(clip, volume);
}

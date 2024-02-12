using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    private AudioSource _audiosource;
    public AudioMixer gameAudioMixer;
    public AudioSource musicSource;


    private void Awake()
    {
        _audiosource = GetComponent<AudioSource>();
        musicSource = GetComponent<AudioSource>();
        PlayBGMusic();
    }
    void PlayBGMusic()
    {
        musicSource.Play();
    }
    void StopBGMusic()
    {
        musicSource.Stop();
    }
    public void SetMusicVolume(float volume)
    {
        gameAudioMixer.SetFloat("MusicVolume", volume); // "MusicVolume" est le nom du paramètre dans l'Audio Mixer
    }
    public void SetFXVolume(float volume)
    {
        gameAudioMixer.SetFloat("FXVolume", volume); // "FXVolume" est le nom du paramètre dans l'Audio Mixer
    }

    /// <summary>
    /// Plays an audio file at max volume.
    /// </summary>
    /// <param name="clip">The AudioClip to play.</param>
    public void PlaySound(AudioClip clip)
    {
        if (_audiosource)
        {
            _audiosource.PlayOneShot(clip, 1f);
        }
    }
    public void StopSound()
    {
        if (_audiosource)
        {
            _audiosource.Stop();
        }
    }

    void PlayMusic()
    {
        // Démarre la lecture de la musique
        musicSource.Play();
    }

    void StopMusic()
    {
        // Arrête la lecture de la musique
        musicSource.Stop();
    }


    /// <summary>
    /// Plays an audio file with a set volume.
    /// </summary>
    /// <param name="clip">The AudioClip to play.</param>
    /// <param name="volume">The volume of the AudioClip to play.</param>
    public void PlaySoundWithVolume(AudioClip clip, float volume)
    {
        if (_audiosource)
        {
            _audiosource.PlayOneShot(clip, volume);
        }
    }
}

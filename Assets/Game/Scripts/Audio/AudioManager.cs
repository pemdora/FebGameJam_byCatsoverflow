using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private AudioMixer _gameAudioMixer;
    [SerializeField] private AudioSource _musicAudioSource;
    [SerializeField] private AudioSource _sfxAudioSource;

    private float _musicBaseVolume = -12f;
    private float _sfxBaseVolume = -12f;
    private float _masterBaseVolume = -12f;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    void Start()
    {
        _gameAudioMixer.SetFloat("MasterVolume", _masterBaseVolume);
        _gameAudioMixer.SetFloat("MusicVolume", _musicBaseVolume);
        _gameAudioMixer.SetFloat("SFXVolume", _sfxBaseVolume);
    }

    public void PlayMusic(AudioClip music)
    {
        _musicAudioSource.clip = music;
        _musicAudioSource.Play();
    }

    public void StopMusic()
    {
        _musicAudioSource.Stop();
    }

    public void PlaySound(AudioClip sound)
    {
        _sfxAudioSource.PlayOneShot(sound);
    }

    public void MasterVolume(float volume)
    {
        _gameAudioMixer.SetFloat("MasterVolume", VolumeToDecibel(volume));
    }

    public void SetMusicVolume(float volume)
    {
        _gameAudioMixer.SetFloat("MusicVolume", VolumeToDecibel(volume));
    }

    public void SetSoundVolume(float volume)
    {
        _gameAudioMixer.SetFloat("SFXVolume", VolumeToDecibel(volume));
    }

    private float VolumeToDecibel(float volume)
    {
        if (volume == 0f)
        {
            return -80f;
        }
        else
        {
            return Mathf.Clamp(
                Mathf.Log10(volume * 20 * 4),
                -80f,
                0f
                );
        }
    }
}
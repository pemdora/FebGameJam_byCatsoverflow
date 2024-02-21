using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    private PlayerSave _playerSave;

    [Header("References")]
    [SerializeField] private AudioMixer _gameAudioMixer;
    [SerializeField] private AudioSource _musicAudioSource;
    [SerializeField] private AudioSource _sfxAudioSource;

    // @pLeet coucou tu peux mettre ça dans le GameManager stp ?
    [Header("GlobalSFX")]
    [SerializeField] private AudioClip _droppedWareSfx;
    [SerializeField] private AudioClip _overSfx;
    [SerializeField] private AudioClip _clicSfx;

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

        _playerSave = SaveManager.Load();
    }

    void Start()
    {
        _gameAudioMixer.SetFloat("MasterVolume", VolumeToDecibel(_playerSave.masterVolume));
        _gameAudioMixer.SetFloat("MusicVolume", VolumeToDecibel(_playerSave.musicVolume));
        _gameAudioMixer.SetFloat("SFXVolume", VolumeToDecibel(_playerSave.soundVolume));
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

    // FOR THE TROLL

    public void PlayOuch()
    {
        _sfxAudioSource.PlayOneShot(_droppedWareSfx);
    }
    public void PlayOver()
    {
        _sfxAudioSource.PlayOneShot(_overSfx);
    }
    public void PlayClic()
    {
        _sfxAudioSource.PlayOneShot(_clicSfx);
    }

    public void SetMasterVolume(float volume)
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
        if (volume <= 0f)
        {
            return -80f;
        }
        else
        {
            return Mathf.Clamp(20 * Mathf.Log10(volume), -80f, 0f);
        }
    }

}
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

    // put in the scriptable objects the sounds and music to play
    [SerializeField] private AudioScriptableObject audioScriptableObject;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        _playerSave = SaveManager.Load();
    }

    private void Start()
    {
        _gameAudioMixer.SetFloat("MasterVolume", VolumeToDecibel(_playerSave.masterVolume));
        _gameAudioMixer.SetFloat("MusicVolume", VolumeToDecibel(_playerSave.musicVolume));
        _gameAudioMixer.SetFloat("SFXVolume", VolumeToDecibel(_playerSave.soundVolume));
    }

    public void PlayMusic(MusicType musicType)
    {
        _musicAudioSource.clip = audioScriptableObject.musicsBySoundType[musicType];
        _musicAudioSource.Play();
    }

    public void StopMusic()
    {
        _musicAudioSource.Stop();
    }

    public void PlaySoundEffect(SoundEffectType soundEffectType)
    {
        _sfxAudioSource.PlayOneShot(audioScriptableObject.soundsEffectsBySoundType[soundEffectType]);
    }
    
    public void PlaySoundEffect(AudioClip audioClip)
    {
        _sfxAudioSource.PlayOneShot(audioClip);
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

        return Mathf.Clamp(20 * Mathf.Log10(volume), -80f, 0f);
    }
}
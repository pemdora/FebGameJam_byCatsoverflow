using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float _timeBeforeWarning;
    public float TimeBeforeWarning => _timeBeforeWarning;

    [Header("References")]
    [SerializeField] private LandingPlatform _landingPlatform;
    [SerializeField] private ScoreManager _scoreManager;
    [SerializeField] private PickManager _pickManager;
    [SerializeField] private SpaceshipManager _spaceshipManager;
    [SerializeField] private MainMenuManager _mainMenuManager;

    [SerializeField] private TimerUI timerUI;

    public UnityEvent OnGameOverEvent;

    private void Start()
    {
        _landingPlatform.CanRotate = false;
        _pickManager.CanPick = false;

        AudioManager.Instance.PlayMusic(MusicType.MENU);

        _scoreManager.OnGameOver.AddListener(OnGameOver);
    }

    private void OnDestroy()
    {
        _scoreManager.OnGameOver.RemoveListener(OnGameOver);
    }

    public void StartGame()
    {
        _scoreManager.enabled = true;
        _pickManager.CanPick = true;
        _spaceshipManager.CanSpawnSpaceship = true;
        
        _spaceshipManager.BringNewSpaceship();

        AudioManager.Instance.PlayMusic(MusicType.IN_GAME);
        timerUI.InitTimer();
    }

    public void SendSpaceship()
    {
        if (!_spaceshipManager.HasSpaceship)
        {
            return;
        }

        _spaceshipManager.SpaceshipDeparture();
    }

    private void OnGameOver()
    {
        _landingPlatform.CanRotate = false;
        _mainMenuManager.ShowGameOver(_scoreManager.Score, _scoreManager.DeliveryCount);
        _scoreManager.ResetData();
        _scoreManager.enabled = false;
        _pickManager.CanPick = false;

        _spaceshipManager.CanSpawnSpaceship = false;
        if (_spaceshipManager.HasSpaceship)
        {
            _spaceshipManager.SpaceshipDeparture();
        }
        _spaceshipManager.Reset();

        AudioManager.Instance.PlaySoundEffect(SoundEffectType.OUTCH);
        AudioManager.Instance.PlayMusic(MusicType.DEFEAT);
        timerUI.StopTimer();
        OnGameOverEvent.Invoke();
    }
}
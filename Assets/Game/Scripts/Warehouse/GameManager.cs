using Game.Scripts.Audio;
using Game.Scripts.Menu;
using Game.Scripts.Score;
using Game.Scripts.Spaceship;
using Game.Scripts.Wares;
using UnityEngine;

namespace Game.Scripts.Warehouse {
    public class GameManager : MonoBehaviour {
        [Header("Settings")]
        [SerializeField] private float _timeBeforeWarning; 
        public float TimeBeforeWarning => _timeBeforeWarning;

        [Header("References")]
        [SerializeField] private LandingPlatform _landingPlatform;
        [SerializeField] private ScoreManager _scoreManager;
        [SerializeField] private PickManager _pickManager;
        [SerializeField] private SpaceshipManager _spaceshipManager;
        [SerializeField] private MainMenuManager _mainMenuManager;
        
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
            _pickManager.CanPick = true;
            _spaceshipManager.CanSpawnSpaceship = true;
            _spaceshipManager.BringNewSpaceship();
            
            AudioManager.Instance.PlayMusic(MusicType.IN_GAME);
        }

        public void SendSpaceship()
        {
            if (!_spaceshipManager.HasSpaceship)
            {
                return;
            }
        
            _spaceshipManager.SpaceshipDeparture();
        }

        private void OnGameOver() {
            _landingPlatform.CanRotate = false;
            _scoreManager.ResetData();
            _pickManager.CanPick = false;
        
            _spaceshipManager.CanSpawnSpaceship = false;

            _mainMenuManager.ShowGameOver();
            AudioManager.Instance.PlaySoundEffect(SoundEffectType.OUTCH);
            AudioManager.Instance.PlayMusic(MusicType.DEFEAT);
        }
    }
}
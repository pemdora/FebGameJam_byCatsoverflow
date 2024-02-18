using System;
using UnityEngine;

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


    private void Start()
    {
        _landingPlatform.CanRotate = false;
        _pickManager.CanPick = false;
        
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
        _scoreManager.ResetData();
        _pickManager.CanPick = false;
        
        _spaceshipManager.CanSpawnSpaceship = false;
        if (_spaceshipManager.HasSpaceship)
        {
            _spaceshipManager.SpaceshipDeparture();
        }

        _mainMenuManager.ShowGameOver();
    } 
}
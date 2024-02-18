using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private LandingPlatform _landingPlatform;
    [SerializeField] private ScoreManager _scoreManager;
    [SerializeField] private PickManager _pickManager;
    [SerializeField] private SpaceshipManager _spaceshipManager;

    private void Start()
    {
        _landingPlatform.CanRotate = false;
        _pickManager.CanPick = false;
    }

    public void StartGame()
    {
        _pickManager.CanPick = true;
        _spaceshipManager.BringNewSpaceship();
    }
}
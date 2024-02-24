using System;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Score calculation
/// Rules :
/// - Each filled slot will generate points (when a ware is placed)
/// - Each empty slot past the given percentage will generate frustration
/// - Each discarded ware will generate frustration
/// </summary>
public class ScoreManager : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private int _deliveryCount;
    [SerializeField] private int _frustration;
    [SerializeField] private int _score;
    
    [Header("Settings")]
    [SerializeField] private ScoreSettings _settings;
    [SerializeField] private ScoreDisplayUI _scoreDisplayUI; // Display Score for ware

    [Header("References")]
    [SerializeField] private SpaceshipManager _spaceshipManager;
    [SerializeField] private FrustrationUI _frustrationUI;

    [Header("Events")]
    public UnityEvent OnGameOver;
    public UnityEvent<string> OnScoreChanged;
    public UnityEvent<int> OnFrustrationThresholdChanged;
    public UnityEvent<bool> OnCargoReachedMinimumRequirement;

    public int DeliveryCount => _deliveryCount;
    public int Frustration => _frustration;
    public int Score => _score;
    public ScoreSettings Settings => _settings;

    private int frustrationThreshold; // dynamic treshold based on cargo
    public int FrustrationThreshold
    {
        get => frustrationThreshold;
        set => frustrationThreshold = value;
    }

    private void OnEnable()
    {
        _spaceshipManager.OnSpaceshipTakeOff.AddListener(OnSpaceshipLeft);
    }

    private void OnDisable()
    {
        _spaceshipManager.OnSpaceshipTakeOff.RemoveListener(OnSpaceshipLeft);
    }

    public void SetObjectiveTreshold(int cargoSize)
    {
        if (cargoSize == 3)
        {
            int frustration = _settings.frustrationThresholdCargo3Min + _deliveryCount*_settings.frustrationThresholdStep;
            frustrationThreshold = Mathf.Min(frustration,_settings.frustrationThresholdCargo3Max);
        }
        else if (cargoSize == 4)
        {
            int frustration = _settings.frustrationThresholdCargo4Min + _deliveryCount*_settings.frustrationThresholdStep;
            frustrationThreshold = Mathf.Min(frustration,_settings.frustrationThresholdCargo4Max) ;
        }
        else
        {
            frustrationThreshold = _settings.frustrationThresholdCargo3Min;
        }

        // _objectiveSlider.value = (100 - frustrationThreshold) / 100f;
        OnFrustrationThresholdChanged?.Invoke(frustrationThreshold);
    }

    private void OnSpaceshipLeft(Spaceship spaceship)
    {
        if (spaceship == null)
        {
            return;
        }

        Cargo cargo = spaceship.Cargo;
        int minimumOccupiedSlotsNeeded = Mathf.CeilToInt(cargo.SlotCount * (frustrationThreshold / 100f));
        bool minimumOccupiedSlotsReached = cargo.FillPercentage > frustrationThreshold;
        int numberOfOccupiedSlotsAboveThreshold = Mathf.Max(0, cargo.OccupiedSlotCount - minimumOccupiedSlotsNeeded);

        // TODO BONUS SCORE
        int extraTresholdBonusPoint = numberOfOccupiedSlotsAboveThreshold * _settings.pointsPerExtraSlotFilled;
        if(extraTresholdBonusPoint > 0)
        {
            _scoreDisplayUI.DisplayExtraThresholdBonus(extraTresholdBonusPoint);
            _score += extraTresholdBonusPoint;
        }

        // If the number of empty slots are above the allowed threshold
        if (!minimumOccupiedSlotsReached)
        {
            // We add frustration for each empty slots
            _frustration += cargo.EmptySlotCount * _settings.frustrationPerEmptySlots;

            // We call the UI dedicated to display the frustration and we update the Filler Image
            _frustrationUI.UpdateFiller((float)_frustration / _settings.maxFrustrationAllowed);

            // If the frustration reach the maximum value, trigger game over
            if (_frustration >= _settings.maxFrustrationAllowed)
            {
                OnGameOver?.Invoke();
                return;
            }
        }
        // else, the spaceship has enough ware in his cargo
        else
        {
            _frustration -= _settings.frustrationRelief;
            _frustration = Mathf.Max(0, _frustration);

            // Check if the player manually send the spaceship
            if (spaceship.LoadingLeft > 0)
            {
                int timerBonus = Mathf.CeilToInt(spaceship.LoadingLeft) * _settings.pointsForEachSecondBeforeEndTimer;
                _scoreDisplayUI.DisplayTimerBonus(timerBonus);
                _score += timerBonus;
            }
        }

        OnCargoReachedMinimumRequirement?.Invoke(minimumOccupiedSlotsReached);
        OnScoreChanged?.Invoke(_score.ToString());
        _deliveryCount++;
    }


    public void PlaceWare(Ware ware)
    {
        int wareScore = ware.Size * _settings.pointsPerSlotFilled;
        // ware.DisplayWareScore(wareScore, ware.BonusScore);
        _scoreDisplayUI.DisplayWareScore(wareScore, ware.transform.position, _settings);
        _score += wareScore + ware.BonusScore;
        OnScoreChanged?.Invoke(_score.ToString());
    }
    

    public void DiscardWare()
    {
        _frustration += _settings.frustrationPerDiscardedWare;

        // We call the UI dedicated to display the frustration and we update the Filler Image
        _frustrationUI.UpdateFiller((float)_frustration / _settings.maxFrustrationAllowed);

        // If the frustration reach the maximum value, trigger game over
        if (_frustration >= _settings.maxFrustrationAllowed)
        {
            OnGameOver?.Invoke();
            return;
        }
    }

    public void ResetData()
    {
        _deliveryCount = 0;
        _frustration = 0;
        _score = 0;
        _frustrationUI.UpdateFiller(0);
    }
}


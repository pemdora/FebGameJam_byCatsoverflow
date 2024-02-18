using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [Header("Stats")] 
    [SerializeField] private int _deliveryCount;
    [SerializeField] private float _frustration;
    [SerializeField] private int _score;

    [Header("Settings")] 
    [SerializeField] private ScoreSettings _settings;

    [Header("References")] 
    [SerializeField] private SpaceshipManager _spaceshipManager;
    
    public int DeliveryCount => _deliveryCount;
    public float Frustration => _frustration;
    public int Score => _score;

    private void OnEnable()
    {
        _spaceshipManager.OnSpaceshipTakeOff.AddListener(OnSpaceshipLeft);
    }

    private void OnDisable()
    {
        _spaceshipManager.OnSpaceshipTakeOff.RemoveListener(OnSpaceshipLeft);
    }

    private void OnSpaceshipLeft(Spaceship spaceship)
    {
        Cargo cargo = spaceship.Cargo;
        int minimumOccupiedSlotsNeeded = Mathf.CeilToInt(cargo.SlotCount * (_settings.frustrationThreshold / 100f));
        bool minimumOccupiedSlotsReached = 100f - cargo.FillPercentage < _settings.frustrationThreshold;
        int numberOfOccupiedSlotsUnderThreshold = Mathf.Min(cargo.OccupiedSlotCount, minimumOccupiedSlotsNeeded);
        int numberOfOccupiedSlotsAboveThreshold = Mathf.Max(0, cargo.OccupiedSlotCount - minimumOccupiedSlotsNeeded);

        _score += numberOfOccupiedSlotsUnderThreshold * _settings.pointsPerSlotFilled;
        _score += numberOfOccupiedSlotsAboveThreshold * _settings.pointsPerExtraSlotFilled;
        
        // If the number of empty slots are above the allowed threshold
        if (!minimumOccupiedSlotsReached)
        {
            // We add frustration for each empty slots
            _frustration += cargo.EmptySlotCount * _settings.frustrationPerEmptySlots;

            // If the frustration reach the maximum value, trigger game over
            if (_frustration >= _settings.maxFrustrationAllowed)
            {
                OnOverFrustration();
            }
        }
        // else, the spaceship has enough ware in his cargo
        else
        {
            // Check if the player manually send the spaceship
            if (spaceship.LoadingLeft > 0)
            {
                _score += Mathf.CeilToInt(spaceship.LoadingLeft) * _settings.pointsForEachSecondBeforeEndTimer;
            }
        }
        
        _deliveryCount++;
    }

    private void OnOverFrustration()
    {
        
    }
}

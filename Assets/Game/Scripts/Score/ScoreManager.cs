using Game.Scripts.HUD;
using Game.Scripts.Spaceship;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Scripts.Score {
    public class ScoreManager : MonoBehaviour
    {
        [Header("Stats")]
        [SerializeField] private int _deliveryCount;
        [SerializeField] private int _frustration;
        [SerializeField] private int _score;

        [Header("Settings")]
        [SerializeField] private ScoreSettings _settings;

        [Header("References")]
        [SerializeField] private SpaceshipManager _spaceshipManager;
        [SerializeField] private FrustrationUI _frustrationUI;

        [Header("Events")]
        public UnityEvent OnGameOver;

        public int DeliveryCount => _deliveryCount;
        public int Frustration => _frustration;
        public int Score => _score;

        private void OnEnable()
        {
            _spaceshipManager.OnSpaceshipTakeOff.AddListener(OnSpaceshipLeft);
        }

        private void OnDisable()
        {
            _spaceshipManager.OnSpaceshipTakeOff.RemoveListener(OnSpaceshipLeft);
        }

        private void OnSpaceshipLeft(Spaceship.Spaceship spaceship)
        {
            Cargo.Cargo cargo = spaceship.Cargo;
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

                // We call the UI dedicated to display the frustration and we update the Filler Image
                _frustrationUI.UpdateFiller((float)_frustration / _settings.maxFrustrationAllowed);

                // If the frustration reach the maximum value, trigger game over
                if (_frustration >= _settings.maxFrustrationAllowed)
                {
                    OnGameOver?.Invoke();
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

        public void ResetData()
        {
            _deliveryCount = 0;
            _frustration = 0;
            _score = 0;
        }
    }
}

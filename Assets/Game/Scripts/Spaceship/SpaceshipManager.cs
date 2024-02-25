using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SpaceshipManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private List<Spaceship> _spaceshipsPrefab;
    [SerializeField] private float _durationBeforeDeparture = 0.75f;

    [Header("References")]
    [SerializeField] private ScoreManager _scoreManager;
    [SerializeField] private LandingPlatform _landingPlatform;
    [SerializeField] private ConveyorStart _conveyorStart;
    [SerializeField] private SpaceshipConductor _arrivalConductor;
    [SerializeField] private SpaceshipConductor _departureConductor;

    [Header("Events")]
    public UnityEvent<Spaceship> OnSpaceshipLanded;
    public UnityEvent<Spaceship> OnSpaceshipTakeOff;

    public bool CanSpawnSpaceship { get; set; }
    public bool HasSpaceship => _currentSpaceship != null;
    public float TimeRemaining => _currentSpaceship.LoadingLeft;
    public float Percentage
    {
        get
        {
            if (_currentSpaceship != null)
            {
                return _currentSpaceship.Cargo.FillPercentage;
            }
            else
            {
                return 0;
            }
        }
        private set
        {

        }

    }

    private Spaceship _currentSpaceship;

    private void Update()
    {
        if (!_currentSpaceship)
        {
            return;
        }

        // Make the spaceship leave if there is no time left on the counter
        if (!_currentSpaceship.HasLeft && _currentSpaceship.LoadingLeft <= 0)
        {
            OnCurrentSpaceshipTimerReachedZero();
            return;
        }

        // Make the spaceship leave if its cargo is full
        if (!_currentSpaceship.HasLeft && _currentSpaceship.Cargo.FillPercentage >= 100)
        {
            _landingPlatform.CanRotate = false;
            OnCurrentSpaceshipFull();
            return;
        }

        // Block landing platform rotation if the spaceship is about to leave
        if (_landingPlatform.CanRotate && _currentSpaceship.LoadingLeft <= _landingPlatform.Duration)
        {
            _landingPlatform.CanRotate = false;
        }
    }

    internal void BringNewSpaceship()
    {
        Spaceship spaceship = GetSpaceship(_spaceshipsPrefab[Random.Range(0, _spaceshipsPrefab.Count)]);
        _scoreManager.SetObjectiveTreshold(spaceship.Cargo.CargoSize); // Set score objective based on the cargo size
        spaceship.gameObject.SetActive(false);
        spaceship.Initialize(_scoreManager.DeliveryCount * _scoreManager.Settings.spaceshipLoadingTimeDecrease / 100 * spaceship.LoadingDuration);
        _arrivalConductor.AttachSpaceship(spaceship, NewSpaceshipLanded, true);
        spaceship.gameObject.SetActive(true);
        spaceship.Cargo.DeactivateCargo();

        _conveyorStart.StartConveyor(spaceship.Cargo.AllowedCollection);
        AudioManager.Instance.PlaySoundEffect(SoundEffectType.TRUCK_ARRIVE);
    }

    private void NewSpaceshipLanded(Spaceship spaceship)
    {
        _landingPlatform.PlaceSpaceship(spaceship);
        _currentSpaceship = spaceship;
        _currentSpaceship.StartLoading();
        _currentSpaceship.Cargo.ActivateCargo();
        _landingPlatform.CanRotate = true;

        OnSpaceshipLanded?.Invoke(spaceship);
    }

    internal void SpaceshipDeparture(bool isCargoFull = false)
    {
        if (!_currentSpaceship)
        {
            return;
        }
        _currentSpaceship.HasLeft = true;
        _currentSpaceship.StopLoading();
        _currentSpaceship.Cargo.DeactivateCargo();

        float speedIncrease = _scoreManager.Settings.GetConveyorBeltSpeedIncrease();
        _conveyorStart.StopConveyor();
        _conveyorStart.ChangeSpeed(_conveyorStart.Speed * speedIncrease, speedIncrease);

        StartCoroutine(DelayBeforeDeparture(isCargoFull, _durationBeforeDeparture));
    }

    private IEnumerator DelayBeforeDeparture(bool isCargoFull, float delay)
    {
        if (isCargoFull)
        {
            if (_currentSpaceship.Cargo.CargoCompletedParticles != null)
                _currentSpaceship.Cargo.CargoCompletedParticles.Play();
            _scoreManager.DisplayPerfectBonus(_currentSpaceship.Cargo.CargoSize);
            yield return new WaitForSeconds(_durationBeforeDeparture);
        }
        else
            yield return null;

        _landingPlatform.ResetRotation(SpaceshipTakeoff);
        AudioManager.Instance.PlaySoundEffect(SoundEffectType.TRUCK_REPART);
    }

    public void DoReset()
    {
        if (_currentSpaceship)
        {
            _currentSpaceship.HasLeft = true;
            _currentSpaceship.StopLoading();
            _currentSpaceship.Cargo.DeactivateCargo();
        }

        _arrivalConductor.DoReset();
        _landingPlatform.ResetRotation(() =>
        {
            _departureConductor.AttachSpaceship(_currentSpaceship, _ => _currentSpaceship = null, false);
        });
        
        _conveyorStart.ResetSpeed();
    }

    public void ReturnSpaceship()
    {
        if (_currentSpaceship)
        {
            ReturnSpaceship(_currentSpaceship);
            _currentSpaceship = null;
        }
    }

    private void SpaceshipTakeoff()
    {
        if (!_currentSpaceship)
        {
            return;
        }

        OnSpaceshipTakeOff?.Invoke(_currentSpaceship);

        _departureConductor.AttachSpaceship(_currentSpaceship, SpaceshipLeft, false);
        _currentSpaceship = null;
    }

    private void SpaceshipLeft(Spaceship spaceship)
    {
        ReturnSpaceship(spaceship);

        if (CanSpawnSpaceship)
        {
            BringNewSpaceship();
        }
    }

    private void OnCurrentSpaceshipTimerReachedZero()
    {
        SpaceshipDeparture();
    }

    private void OnCurrentSpaceshipFull()
    {
        SpaceshipDeparture(true);
    }

    private Spaceship GetSpaceship(Spaceship spaceship)
    {
        return Instantiate(spaceship.gameObject, Vector3.one * -500, spaceship.transform.rotation).GetComponent<Spaceship>();
    }

    private void ReturnSpaceship(Spaceship spaceship)
    {
        Destroy(spaceship.gameObject);
    }
}

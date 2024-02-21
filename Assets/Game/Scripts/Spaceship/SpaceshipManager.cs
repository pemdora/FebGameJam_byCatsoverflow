using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SpaceshipManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private List<Spaceship> _spaceshipsPrefab;

    [Header("References")]
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
    public float Percentage => _currentSpaceship.Cargo.FillPercentage;

    private Spaceship _currentSpaceship;

    private void Update()
    {
        if (!_currentSpaceship)
        {
            return;
        }

        // Make the spaceship leave if there is no time left on the counter
        if (_currentSpaceship.LoadingLeft <= 0)
        {
            OnCurrentSpaceshipTimerReachedZero();
            return;
        }

        // Make the spaceship leave if its cargo is full
        if (_currentSpaceship.Cargo.FillPercentage >= 100)
        {
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
        spaceship.gameObject.SetActive(false);
        spaceship.Initialize();
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

    internal void SpaceshipDeparture()
    {
        _currentSpaceship.StopLoading();
        _currentSpaceship.Cargo.DeactivateCargo();
        _conveyorStart.StopConveyor();

        _landingPlatform.ResetRotation(SpaceshipTakeoff);
        AudioManager.Instance.PlaySoundEffect(SoundEffectType.TRUCK_REPART);
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
        SpaceshipDeparture();
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

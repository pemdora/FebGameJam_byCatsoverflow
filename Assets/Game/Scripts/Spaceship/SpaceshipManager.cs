using System.Collections.Generic;
using UnityEngine;

public class SpaceshipManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private List<Spaceship> _spaceshipsPrefab;
    
    [Header("References")]
    [SerializeField] private LandingPlatform _landingPlatform;
    [SerializeField] private ConveyorStart _conveyorStart;
    [SerializeField] private SpaceshipConductor _arrivalConductor;
    [SerializeField] private SpaceshipConductor _departureConductor;

    private Spaceship _currentSpaceship;
    public bool IsAvailable => _currentSpaceship!=null;
    public float TimeRemaining => _currentSpaceship.LoadingLeft;
    public float Percentage => _currentSpaceship.Cargo.FillPercentage;

    // Start is called before the first frame update
    void Start()
    {
        BringNewSpaceship();
    }

    // Update is called once per frame
    void Update()
    {
        if (!_currentSpaceship)
        {
            return;
        }
        
        // Make the spaceship leave if there is no time left on the counter
        if (_currentSpaceship.LoadingLeft <= 0)
        {
            OnCurrentSpaceshipTimerReachedZero();
        }
        
        // Block landing platform rotation if the spaceship is about to leave
        if (_landingPlatform.CanRotate && _currentSpaceship.LoadingLeft <= _landingPlatform.Duration)
        {
            _landingPlatform.CanRotate = false;
        }
    }

    private void BringNewSpaceship()
    {
        Spaceship spaceship = GetSpaceship(_spaceshipsPrefab[Random.Range(0, _spaceshipsPrefab.Count)]);
        spaceship.Initialize();
        _arrivalConductor.AttachSpaceship(spaceship, NewSpaceshipLanded, true);
        spaceship.gameObject.SetActive(true);
        spaceship.Cargo.DeactivateCargo();
        
        _conveyorStart.StartConveyor(spaceship.Cargo.AllowedCollection);
    }

    private void NewSpaceshipLanded(Spaceship spaceship)
    {
        _landingPlatform.PlaceSpaceship(spaceship);
        _currentSpaceship = spaceship;
        _currentSpaceship.StartLoading();
        _currentSpaceship.Cargo.ActivateCargo();
        _landingPlatform.CanRotate = true;
    }

    public void SpaceshipDeparture()
    {
        _landingPlatform.ResetRotation(SpaceshipTakeoff);
    }

    private void SpaceshipTakeoff()
    {
        _currentSpaceship.StopLoading();
        _currentSpaceship.Cargo.DeactivateCargo();
        _conveyorStart.StopConveyor();
        
        _departureConductor.AttachSpaceship(_currentSpaceship, SpaceshipLeft, false);
        _currentSpaceship = null;
    }

    private void SpaceshipLeft(Spaceship spaceship)
    {
        ReturnSpaceship(spaceship);
        BringNewSpaceship();
    }

    private void OnCurrentSpaceshipTimerReachedZero()
    {
        // TODO : decrease score then check if loose condition is reached. If not reached, we make the spaceship left
        SpaceshipDeparture();
    }

    private void OnCurrentSpaceshipFull()
    {
        // TODO : increase score
        SpaceshipDeparture();
    }
    
    private Spaceship GetSpaceship(Spaceship spaceship)
    {
        return Instantiate(spaceship.gameObject).GetComponent<Spaceship>();
    }

    private void ReturnSpaceship(Spaceship spaceship)
    {
        Destroy(spaceship.gameObject);
    }
}

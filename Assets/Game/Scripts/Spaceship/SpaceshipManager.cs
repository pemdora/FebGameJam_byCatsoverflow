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
    private Dictionary<int, List<Spaceship>> _spaceshipsPool;

    // Start is called before the first frame update
    void Start()
    {
        _spaceshipsPool = new();
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
        
        // TODO update remaining time in world digetic
    }

    private void BringNewSpaceship()
    {
        Spaceship spaceship = GetSpaceship(_spaceshipsPrefab[Random.Range(0, _spaceshipsPrefab.Count)]);
        _arrivalConductor.AttachSpaceship(spaceship, NewSpaceshipLanded);
        
        _conveyorStart.StartConveyor(spaceship.Cargo.AllowedCollection);
    }

    private void NewSpaceshipLanded(Spaceship spaceship)
    {
        _landingPlatform.PlaceSpaceship(spaceship);
        _currentSpaceship = spaceship;
        _currentSpaceship.StartLoading();
        _landingPlatform.CanRotate = true;
    }

    private void SpaceshipDeparture()
    {
        _currentSpaceship.StopLoading();
        _conveyorStart.StopConveyor();
        
        _departureConductor.AttachSpaceship(_currentSpaceship, SpaceshipLeft);
        _currentSpaceship = null;
    }

    private void SpaceshipLeft(Spaceship spaceship)
    {
        ReturnSpaceship(spaceship);
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
        Spaceship instance;
        int id = spaceship.id;
        if (_spaceshipsPool.ContainsKey(id) && _spaceshipsPool[id]?.Count > 0)
        {
            instance = _spaceshipsPool[id][0];
            _spaceshipsPool[id].RemoveAt(0);
        }
        else
        {
            instance = Instantiate(spaceship.gameObject).GetComponent<Spaceship>();
        }

        instance.Initialize();
        return instance;
    }

    private void ReturnSpaceship(Spaceship spaceship)
    {
        int id = spaceship.id;
        if (!_spaceshipsPool.ContainsKey(id))
        {
            _spaceshipsPool.Add(id, new List<Spaceship>());
        }

        if (_spaceshipsPool[id] == null)
        {
            _spaceshipsPool[id] = new List<Spaceship>();
        }
        
        _spaceshipsPool[id].Add(spaceship);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SpaceshipManager : MonoBehaviour
{
    [SerializeField] private LandingPlatform _landingPlatform;
    [SerializeField] private List<Spaceship> _spaceshipPrefabList;


    private Spaceship _spaceship;
    private Queue<Spaceship> _spaceshipPrefabQueue;
    private float currentTime = 0;
    private int _spaceshipTime = 0;
    private Coroutine _arrivalCoroutine;
    private Coroutine _leaveCoroutine;


    // Start is called before the first frame update
    void Start()
    {
        _spaceshipPrefabQueue = new Queue<Spaceship>();
        for (int i = 0; i < _spaceshipPrefabList.Count; i++)
        {
            _spaceshipPrefabQueue.Enqueue(_spaceshipPrefabList[i]);
        }
        if (_landingPlatform)
        {
            _spaceship = _landingPlatform.GetComponentInChildren<Spaceship>();
        }
        if (_spaceship)
        {
            _spaceshipTime = _spaceship.Duration;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.I))
        {
            NextSpaceship();
        }

        if (_spaceship)
        {
            currentTime += Time.deltaTime;
            if (currentTime > _spaceshipTime)
            {
                NextSpaceship();
            }
            if (_landingPlatform && SpaceshipRemainingTime() < _landingPlatform.Duration)
            {
                _landingPlatform.CanRotate = false;
            }
        }

    }

    void NextSpaceship()
    {
        if (_leaveCoroutine == null && _arrivalCoroutine == null)
        {
            _spaceshipTime = 0;
            _leaveCoroutine = StartCoroutine(DestroySpaceshipCoroutine());
            _arrivalCoroutine = StartCoroutine(SpaceshipArrival());
        }
    }


    Spaceship SpawnNextSpaceship()
    {
        if (_spaceshipPrefabQueue.Count > 0)
        {
            Spaceship spaceshipPrefab = _spaceshipPrefabQueue.Dequeue();
            if (spaceshipPrefab)
            {
                Spaceship spaceshipObject = Instantiate(spaceshipPrefab);
                spaceshipObject.transform.position = _landingPlatform.transform.position;
                spaceshipObject.DeactivateCargo();
                return spaceshipObject;
            }
            return null;

        }
        else
        {
            return null;
        }
    }

    private IEnumerator DestroySpaceshipCoroutine()
    {
        Spaceship spaceshipToDestroy = _spaceship;
        if (spaceshipToDestroy)
        {
            spaceshipToDestroy.DeactivateCargo();
            spaceshipToDestroy.transform.parent = null;
        }
        float percent = 0;
        float _duration = 1.0f;
        while (percent < 1)
        {
            percent += Time.deltaTime * 1 / _duration;
            spaceshipToDestroy.transform.position += new Vector3(15 * Time.deltaTime, 0, 0);
            yield return null;
        }

        Destroy(spaceshipToDestroy.gameObject);
        _leaveCoroutine = null;
    }

    private IEnumerator SpaceshipArrival()
    {
        Spaceship nextSpaceship = SpawnNextSpaceship();
        if (nextSpaceship)
        {
            nextSpaceship.transform.position -= new Vector3(15, 0, 0);

            float percent = 0;
            float _duration = 1.0f;
            while (percent < 1)
            {
                percent += Time.deltaTime * 1 / _duration;
                nextSpaceship.transform.position += new Vector3(15 * Time.deltaTime, 0, 0);
                yield return null;

            }
            nextSpaceship.ActivateCargo();
            _spaceship = nextSpaceship;
            _spaceship.transform.SetParent(_landingPlatform.transform);
            _spaceshipTime = _spaceship.Duration;
            _arrivalCoroutine = null;
            currentTime = 0;
            _landingPlatform.CanRotate = true;
        }

    }

    public float SpaceshipRemainingTime()
    {
        return _spaceshipTime - currentTime;
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorStart : MonoBehaviour
{
    private float BASE_SPEED = 1.85f; // vitesse de base synchro avec l'animation

    [Header("Settings")]
    [SerializeField] private float _speed;
    [SerializeField] private float _spawnDelay;
    [SerializeField] private float _spawnInterval;

    [Header("References")]
    [SerializeField] private ConveyorItem _conveyorSlotPrefab;
    [SerializeField] private WareCollection _wareCollection;
    [SerializeField] private Transform _warePoolsContainer;

    [Header("Animation")]
    [SerializeField] private Animator _beltAnimator;
    [SerializeField] private GameObject _belt;

    public bool IsRunning { get; private set; }

    private Transform _transform;
    private Coroutine _spawningCoroutine;
    private List<ConveyorItem> _tracked;
    private List<ConveyorItem> _pool;
    private Dictionary<int, List<Ware>> _warePools;

    private void Awake()
    {
        _transform = transform;
        _tracked = new List<ConveyorItem>();
        _pool = new List<ConveyorItem>();
        // on modifie la vitesse de l'animation pour qu'elle corresponde à la vitesse de base et au scale du tapis ( la BASE_SPEED est la vitesse de base de l'animation pour un scale de 1)
        _beltAnimator.speed = (_speed / BASE_SPEED) / _belt.transform.localScale.x;    
        InitializeWarePools();
    }

    private void Start()
    {
        // TODO: let level manager handle this
        StartConveyor();
    }

    /// <summary>
    /// Initializes the pooling of wares into a dictionary.
    /// </summary>
    private void InitializeWarePools()
    {
        if (_warePools != null)
        {
            return;
        }

        _warePools = new();

        int count = 0;

        foreach (Ware ware in _wareCollection.wares)
        {
            _warePools.Add(count, new List<Ware>());
            count++;
        }
    }

    public void StartConveyor()
    {
        if (IsRunning)
        {
            return;
        }

        IsRunning = true;
        _spawningCoroutine = StartCoroutine(SpawningCoroutine());
    }

    public void StopConveyor()
    {
        if (!IsRunning)
        {
            return;
        }

        IsRunning = false;
        StopCoroutine(_spawningCoroutine);
        _spawningCoroutine = null;
    }

    private IEnumerator SpawningCoroutine()
    {
        yield return new WaitForSeconds(_spawnDelay);

        while (IsRunning)
        {
            ConveyorItem conveyorItem = GetConveyorItem();
            Ware ware = GetWare();

            conveyorItem.transform.position = _transform.position;
            conveyorItem.Initialize(this, _speed, ware);
            conveyorItem.gameObject.SetActive(true);

            _tracked.Add(conveyorItem);

            yield return new WaitForSeconds(_spawnInterval);
        }

        _spawningCoroutine = null;
    }

    private ConveyorItem GetConveyorItem()
    {
        ConveyorItem conveyorItem;

        if (_pool.Count > 0)
        {
            conveyorItem = _pool[0];
            _pool.RemoveAt(0);
        }
        else
        {
            conveyorItem = Instantiate(_conveyorSlotPrefab, _transform.position, _transform.rotation);
        }

        return conveyorItem;
    }

    private Ware GetWare()
    {
        Ware ware = null;

        if (_warePools.Count == 0)
        {
            Debug.LogError($"Ware list is empty.");
            return null;
        }

        int wareIndex = Random.Range(0, _warePools.Count);

        List<Ware> warePool = _warePools[wareIndex];

        foreach (Ware thisWare in warePool)
        {
            if (!thisWare.gameObject.activeSelf)
            {
                ware = thisWare;
                ware.gameObject.SetActive(true);
                break;
            }
        }

        if (ware == null)
        {
            ware = Instantiate(_wareCollection.wares[wareIndex]);
            warePool.Add(ware);
        }

        ware.Initialize(_warePoolsContainer);
        return ware;
    }

    public void ReturnConveyorItem(ConveyorItem conveyorItem)
    {
        conveyorItem.gameObject.SetActive(false);

        // If the conveyor item has a ware, we clear it
        if (conveyorItem.TryGetWare(out Ware ware))
        {
            conveyorItem.ClearWare();

            ware.gameObject.SetActive(false);
            ware.transform.SetParent(_warePoolsContainer);
        }

        // Stop tracking that conveyor item and put it back into the pool
        _tracked.Remove(conveyorItem);
        _pool.Add(conveyorItem);
    }

    public void ChangeSpeed(float speed)
    {
        _speed = speed;
        _beltAnimator.speed = _speed / BASE_SPEED;

        foreach (ConveyorItem conveyorItem in _tracked)
        {
            conveyorItem.ChangeSpeed(_speed);
        }
    }
}
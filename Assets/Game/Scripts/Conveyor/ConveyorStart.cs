using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorStart : MonoBehaviour
{
    [Header("Settings")] 
    [SerializeField] private float _speed;
    [SerializeField] private float _spawnDelay;
    [SerializeField] private float _spawnInterval;

    [Header("References")] 
    [SerializeField] private ConveyorItem _conveyorSlotPrefab;
    [SerializeField] private WareCollection _wareCollection;

    public bool IsRunning { get; private set; }

    private Transform _transform;
    private Coroutine _spawningCoroutine;
    private List<ConveyorItem> _tracked;
    private List<ConveyorItem> _pool;

    private void Awake()
    {
        _transform = transform;
        _tracked = new List<ConveyorItem>();
        _pool = new List<ConveyorItem>();
    }

    private void Start()
    {
        // TODO: let level manager handle this
        StartConveyor();
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
        // TODO: adapt based on difficulty
        Ware ware = Instantiate(_wareCollection.GetRandom());
        ware.Initialize();
        return ware;
    }

    public void ReturnConveyorItem(ConveyorItem conveyorItem)
    {
        conveyorItem.gameObject.SetActive(false);

        // If the conveyor item has a ware, we clear it
        if (conveyorItem.TryGetWare(out Ware ware))
        {
            conveyorItem.ClearWare();
            
            // TODO: change? pool?
            Destroy(ware.gameObject);
        }

        // Stop tracking that conveyor item and put it back into the pool
        _tracked.Remove(conveyorItem);
        _pool.Add(conveyorItem);
    }

    public void ChangeSpeed(float speed)
    {
        _speed = speed;

        foreach (ConveyorItem conveyorItem in _tracked)
        {
            conveyorItem.ChangeSpeed(_speed);
        }
    }
}
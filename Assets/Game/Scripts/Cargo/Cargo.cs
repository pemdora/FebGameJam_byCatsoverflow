using System.Collections.Generic;
using UnityEngine;

public class Cargo : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int _cargoSize = 3;
    [SerializeField] private WareCollection _allowedCollection;
    
    [Header("References")]
    [SerializeField] private CargoSlot[] _slots;
    [SerializeField] private LayerMask _wareLayerMask;

    public WareCollection AllowedCollection => _allowedCollection;
    public float FillPercentage => _fillPercentage;
    public int SlotCount => _slotCount;
    public int OccupiedSlotCount => _occupiedSlotCount;
    public int EmptySlotCount => _emptySlotCount;
    
    private List<Ware> _placedWare;
    private Dictionary<Ware.WareTypes, int> _typesCounter;
    private int _slotCount;
    private float _fillPercentage = 0f;
    private int _occupiedSlotCount;
    private int _emptySlotCount;

    private void Start()
    {
        _typesCounter = new Dictionary<Ware.WareTypes, int>();
        _placedWare = new List<Ware>();
        foreach (CargoSlot cargoSlot in _slots)
        {
            cargoSlot.Initialize(this);
        }

        _slotCount = Mathf.FloorToInt(Mathf.Pow(_cargoSize, 3));
        _emptySlotCount = _slotCount;
        _occupiedSlotCount = 0;
    }

    public void AddWare(Ware ware)
    {
        _placedWare.Add(ware);
        AddWareTypes(ware.GetWareType());
        UpdateCargoContent();

        // TODO: add ware interaction trigger here
    }

    public void RemoveWare(Ware ware)
    {
        _placedWare.Remove(ware);
        RemoveWareTypes(ware.GetWareType());
        UpdateCargoContent();
    }

    public void UpdateCargoContent()
    {
        _occupiedSlotCount = 0;

        foreach (Ware ware in _placedWare)
        {
            _occupiedSlotCount += ware.Size;
        }

        _emptySlotCount = _slotCount - _occupiedSlotCount;
        
        if (_occupiedSlotCount > 0)
        {
            _fillPercentage = ((float)_occupiedSlotCount / (float)_slotCount) * 100f;
        }
    }

    void AddWareTypes(Ware.WareTypes wareType)
    {
        if (!_typesCounter.ContainsKey(wareType))
        {
            _typesCounter[wareType] = 0;
        }
        _typesCounter[wareType]++;
    }

    void RemoveWareTypes(Ware.WareTypes wareType)
    {
        if (!_typesCounter.ContainsKey(wareType))
        {
            _typesCounter[wareType] = 0;
        }
        _typesCounter[wareType]--;
    }

    void DebugPrintTypesCounter()
    {
        string debugString = "Types counter: ";
        Debug.Log(debugString);
        foreach (KeyValuePair<Ware.WareTypes, int> pair in _typesCounter)
        {
            debugString = "[" + pair.Key + "] : " + pair.Value + " ";
            Debug.Log(debugString);
        }
    }

    public void ActivateCargo()
    {
        for (int i = 0; i < _slots.Length; i++)
        {
            Collider[] colliders = _slots[i].GetComponents<Collider>();
            for (int j = 0; j < colliders.Length; j++)
            {
                Collider collider = colliders[j];
                collider.enabled = true;
            }
        }
    }

    public void DeactivateCargo()
    {
        Collider[] hitWares = Physics.OverlapBox(transform.position + new Vector3(0, _cargoSize * 0.5f, 0), new Vector3(_cargoSize, _cargoSize, _cargoSize) * 0.5f, Quaternion.identity, _wareLayerMask);
        for (int i = 0; i < hitWares.Length; i++)
        {
            hitWares[i].enabled = false;
        }
        for (int i = 0; i < _slots.Length; i++)
        {
            Collider[] colliders = _slots[i].GetComponents<Collider>();
            for (int j = 0; j < colliders.Length; j++)
            {
                Collider collider = colliders[j];
                collider.enabled = false;
            }
        }
    }

    public void ResetWares()
    {
        if (_placedWare != null)
        {
            foreach (Ware ware in _placedWare)
            {
                ware.transform.SetParent(ware.WarePoolContainer);
                ware.gameObject.SetActive(false);
            }
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        _slots = GetComponentsInChildren<CargoSlot>();
    }
#endif
}

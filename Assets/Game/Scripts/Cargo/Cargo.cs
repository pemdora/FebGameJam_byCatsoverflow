using System.Collections.Generic;
using UnityEngine;

public class Cargo : MonoBehaviour
{
    [Header("References")] 
    [SerializeField] private CargoSlot[] _slots;
    [SerializeField] private LayerMask _wareLayerMask;
    [SerializeField] private int _cargoSize = 3;

    private float _fillPercentage = 0f;
    private int _cargoCases;
    private List<Ware> _placedWare;
    
    private void Start()
    {
        _placedWare = new List<Ware>();
        foreach (CargoSlot cargoSlot in _slots)
        {
            cargoSlot.Initialize(this);

        }
        _cargoCases = Mathf.FloorToInt(Mathf.Pow(_cargoSize, 3));
    }

    public void AddWare(Ware ware)
    {
        _placedWare.Add(ware);
        UpdateCargoContent();

        // TODO: add ware interaction trigger here
    }

    public void RemoveWare(Ware ware)
    {
        _placedWare.Remove(ware);
        UpdateCargoContent();

    }


    public void UpdateCargoContent()
    {
        Collider[] hitWares = Physics.OverlapBox(transform.position + new Vector3(0, _cargoSize * 0.5f, 0), new Vector3(_cargoSize, _cargoSize, _cargoSize) * 0.5f, Quaternion.identity, _wareLayerMask);
        int casesFilled = hitWares.Length;
        if (casesFilled > 0)
        {
            _fillPercentage = ((float)casesFilled / (float)_cargoCases) * 100f;
            _fillPercentage = Mathf.Round(_fillPercentage);
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
        for (int i = 0; i < hitWares.Length;i++)
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

#if UNITY_EDITOR
    private void OnValidate()
    {
        _slots = GetComponentsInChildren<CargoSlot>();
    }
#endif
}
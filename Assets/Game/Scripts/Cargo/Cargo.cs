using System.Collections.Generic;
using UnityEngine;

public class Cargo : MonoBehaviour
{
    [Header("References")] 
    [SerializeField] private CargoSlot[] _slots;

    private List<Ware> _placedWare;
    
    private void Start()
    {
        _placedWare = new List<Ware>();
        foreach (CargoSlot cargoSlot in _slots)
        {
            cargoSlot.Initialize(this);
        }
    }

    public void AddWare(Ware ware)
    {
        _placedWare.Add(ware);
        
        // TODO: add ware interaction trigger here
    }

    public void RemoveWare(Ware ware)
    {
        _placedWare.Remove(ware);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        _slots = GetComponentsInChildren<CargoSlot>();
    }
#endif
}
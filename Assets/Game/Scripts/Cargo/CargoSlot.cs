using UnityEngine;

public class CargoSlot : MonoBehaviour, IWareSupport
{
    private Cargo _owner;
    
    public void Initialize(Cargo cargo)
    {
        _owner = cargo;
    }
    
    public bool CanSupportWare(Ware ware, LayerMask supportLayerMask)
    {
        return true;
    }

    public Vector3 GetSnapSupportPosition(Ware ware, Vector3 warePosition, Vector3 mouseOffset)
    {
        mouseOffset.y = mouseOffset.y % 1;
        return warePosition + Vector3Int.RoundToInt(mouseOffset);
    }

    public Cargo GetAssociatedCargo()
    {
        return _owner;
    }
}
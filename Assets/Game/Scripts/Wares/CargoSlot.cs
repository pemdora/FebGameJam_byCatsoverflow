using UnityEngine;

public class CargoSlot : MonoBehaviour, IWareSupport
{
    public bool CanSupportWare(Ware ware, LayerMask supportLayerMask)
    {
        return true;
    }

    public Vector3 GetSnapSupportPosition(Ware ware, Vector3 warePosition, Vector3 mouseOffset)
    {
        return Vector3Int.RoundToInt(warePosition + mouseOffset);
    }
}
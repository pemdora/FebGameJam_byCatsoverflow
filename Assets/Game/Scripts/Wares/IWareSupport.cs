using UnityEngine;

public interface IWareSupport
{
    public bool CanSupportWare(Ware ware, LayerMask supportLayerMask);
    public Vector3 GetSnapSupportPosition(Ware ware, Vector3 warePosition, Vector3 hitPosition, Vector3 mouseOffset);
    public Cargo GetAssociatedCargo();
}
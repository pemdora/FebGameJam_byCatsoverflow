using UnityEngine;

namespace Game.Scripts.Wares {
    public interface IWareSupport
    {
        public bool CanSupportWare(Ware ware, LayerMask supportLayerMask);
        public Vector3 GetSnapSupportPosition(Ware ware, Vector3 warePosition, Vector3 mouseOffset);
        public Cargo.Cargo GetAssociatedCargo();
    }
}
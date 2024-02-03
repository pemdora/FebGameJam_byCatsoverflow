using UnityEngine;

public enum WareBoundsSupport
{
    None,
    Self,
    CargoSlot,
    OtherWare,
}

public class WareBounds : MonoBehaviour
{
    [Header("References")] 
    [SerializeField] private Collider _collider;
    [SerializeField] private GameObject _overlapEffect;
    [SerializeField] private GameObject _correctEffect;
    
    private Ware _associateWare;
    
    public Ware GetWare()
    {
        return GetComponentInParent<Ware>();
    }

    public void SetInteractible(bool value)
    {
        _collider.enabled = value;
    }

    public bool DoesOverlap(LayerMask layerMask)
    {
        Collider[] hitColliders = Physics.OverlapBox(transform.position, transform.localScale / (1 / 0.49f), Quaternion.identity, layerMask);

        foreach (Collider collider in hitColliders)
        {
            if (collider.gameObject == gameObject)
            {
                // Don't take into account for self overlap
                continue;
            }

            return true;
        }
        
        return false;
    }

    public WareBoundsSupport GetSupport(LayerMask supperLayerMask)
    {
        Collider[] hitColliders = Physics.OverlapBox(transform.position - Vector3.down, transform.localScale / (1 / 0.49f), Quaternion.identity, supperLayerMask);

        foreach (Collider collider in hitColliders)
        {
            if (collider.gameObject == gameObject)
            {
                // Don't take into account for self overlap, tho it shouldn't happen in this case
                continue;
            }

            // First, check if the collider beneath is a ware 
            Ware colliderWare = collider.GetComponentInParent<Ware>();
            if (colliderWare != null)
            {
                if (colliderWare == _associateWare)
                {
                    return WareBoundsSupport.Self;
                }

                return WareBoundsSupport.OtherWare;
            }
            
            // Then check if the collider beneath is a slot
            CargoSlot colliderCargoSlot = collider.GetComponentInParent<CargoSlot>();
            if (colliderCargoSlot != null)
            {
                return WareBoundsSupport.CargoSlot;
            }
        }
        
        return WareBoundsSupport.None;
    }
}
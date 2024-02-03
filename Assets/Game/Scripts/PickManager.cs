using UnityEngine;

public class PickManager : MonoBehaviour
{
    [Header("Settings")] 
    [SerializeField] private LayerMask _wareLayerMask;
    [SerializeField] private LayerMask _worldLayerMask;
    [SerializeField] private LayerMask _slotLayerMask;
    
    [Header("References")]
    [SerializeField] private Camera _camera;
    
    private Ware _hoveredWare;
    private Ware _selectedWare;
    private Vector3 _selectedWareOffset;

    void Update()
    {
        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);

        // If we have a selected ware...
        if (_selectedWare)
        {
            // .. check if we can place it on a cargo slot or another ware
            if (Physics.Raycast(ray, out RaycastHit slotHit, Mathf.Infinity, _slotLayerMask))
            {
                // If the mouse is over another ware
                Ware colliderWare = slotHit.collider.GetComponentInParent<Ware>();
                if (colliderWare != null)
                {
                    Vector3 clampedOffset = new Vector3(Mathf.RoundToInt(_selectedWareOffset.x), 1, Mathf.RoundToInt(_selectedWareOffset.z));
                    _selectedWare.transform.position = slotHit.transform.position + clampedOffset;
                }
            
                // If the mouse is over a cargo slot
                CargoSlot colliderCargoSlot = slotHit.collider.GetComponentInParent<CargoSlot>();
                if (colliderCargoSlot != null)
                {
                    Vector3 clampedOffset = new Vector3(Mathf.RoundToInt(_selectedWareOffset.x), 0, Mathf.RoundToInt(_selectedWareOffset.z));
                    _selectedWare.transform.position = slotHit.transform.position + clampedOffset;
                }

                // If the player press the mouse
                if (Input.GetMouseButtonUp(0))
                {
                    // We drop the ware at the localisation
                    _selectedWare.SetInteractable(true);
                    _selectedWare = null;
                    
                }
            }
            // ... if we can't, we make it follow the mouse
            else if (Physics.Raycast(ray, out RaycastHit worldHit, Mathf.Infinity, _worldLayerMask))
            {
                _selectedWare.transform.position = new Vector3(worldHit.point.x + _selectedWareOffset.x, _selectedWare.transform.position.y, worldHit.point.z + _selectedWareOffset.z);
            }
        }
        // If we don't have a selected ware, check if we can take one 
        else
        {
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _wareLayerMask))
            {
                if (hit.transform.TryGetComponent(out WareBounds wareBounds))
                {
                    Ware ware = wareBounds.GetWare();
                    
                    // If the ware we're hovering over is different than the previous one, we change the highlighted one
                    if (_hoveredWare != ware)
                    {
                        ClearPreviousHighlight();
                        ActiveHighlight(ware);
                    }

                    // If we clicked the left button, we grab the ware
                    if (Input.GetMouseButtonUp(0))
                    {
                        _selectedWare = ware;
                        _selectedWareOffset = ware.transform.position - hit.point;
                        _selectedWare.SetInteractable(false);

                        ClearPreviousHighlight();
                    }
                }
            }
            // If the mouse do not hover a Ware
            else
            {
                // If a ware was previously highlighted, we disable the highlight
                if (_hoveredWare != null)
                {
                    ClearPreviousHighlight();
                }
            }
        }
    }

    private void ActiveHighlight(Ware ware)
    {
        ClearPreviousHighlight();

        _hoveredWare = ware;
        _hoveredWare.SetHighlight(true);
    }
    
    private void ClearPreviousHighlight()
    {
        if (_hoveredWare == null)
        {
            return;
        }

        _hoveredWare.SetHighlight(false);
        _hoveredWare = null;
    }
}
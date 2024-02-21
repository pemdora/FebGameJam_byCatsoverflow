using UnityEngine;

public class PickManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private LayerMask _wareLayerMask;
    [SerializeField] private LayerMask _worldLayerMask;
    [SerializeField] private LayerMask _supportLayerMask;
    [SerializeField] private LayerMask _obstacleLayerMask;

    [Header("References")]
    [SerializeField] private Camera _camera;
    [SerializeField] private LandingPlatform _landingPlatform;
    [SerializeField] private ScoreManager _scoreManager;

    public bool CanPick { get; set; } = true;

    private Ware _hoveredWare;
    private Ware _selectedWare;
    private Vector3 _selectedWareOffset;

    private IWareSupport support;

    void Update()
    {
        if (!CanPick)
        {
            return;
        }

        bool isWareSnapped = false;
        bool isPlatformRotating = _landingPlatform.IsRotating;
        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);

        // If we have a selected ware...
        if (_selectedWare)
        {
            // .. check if we can place it on a support (cargo slot or another ware)
            if (!isPlatformRotating && Physics.Raycast(ray, out RaycastHit supportHit, Mathf.Infinity, _supportLayerMask))
            {
                support = supportHit.collider.GetComponentInParent<IWareSupport>();
                if (support.CanSupportWare(_selectedWare, _supportLayerMask))
                {
                    _selectedWare.transform.position = support.GetSnapSupportPosition(_selectedWare, supportHit.transform.position, _selectedWareOffset);
                    _selectedWare.UpdateBoundsIndicators();
                    isWareSnapped = true;

                    // If the player press the mouse
                    if (_selectedWare.CanBePlaced(_obstacleLayerMask) && Input.GetMouseButtonUp(0))
                    {
                        // We drop the ware at the location
                        _selectedWare.Place(support.GetAssociatedCargo());
                        _selectedWare = null;
                    }
                }
            }
            // drop the ware if we can't place it
            if (!isWareSnapped && Input.GetMouseButtonUp(0))
            {
                _selectedWare.Drop();
                _selectedWare = null;
                _scoreManager.DiscardWare();
                return;
            }

            // ... if we can't, we make it follow the mouse
            if (!isWareSnapped && Physics.Raycast(ray, out RaycastHit worldHit, Mathf.Infinity, _worldLayerMask))
            {
                _selectedWare.ClearBoundsIndicators();
                _selectedWare.transform.position = new Vector3(worldHit.point.x + _selectedWareOffset.x, _selectedWareOffset.y, worldHit.point.z + _selectedWareOffset.z);
            }

            // Rotate ware if needed
            if (!isPlatformRotating && Input.GetMouseButtonUp(1))
            {
                Vector3 oldOffset = _selectedWareOffset;
                Vector3 newOffset = Quaternion.Euler(0, 90, 0) * oldOffset;
                if (_selectedWare.Rotate(90, newOffset - oldOffset))
                {
                    _selectedWareOffset = newOffset;
                }
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
                    ClearPreviousHighlight();

                    if (!ware.CanBePicked(_wareLayerMask))
                    {
                        // If the ware cannot be picked (is supporting other ware), we do not highlight nor pick it
                        return;
                    }

                    // If the ware we're hovering over is different than the previous one, we change the highlighted one
                    if (_hoveredWare != ware)
                    {
                        ActiveHighlight(ware);
                        AudioManager.Instance.PlaySoundEffect(SoundEffectType.OUTCH);
                    }

                    // If we clicked the left button, we grab the ware
                    if (Input.GetMouseButtonUp(0))
                    {
                        _selectedWare = ware;
                        _selectedWareOffset = ware.transform.position - wareBounds.transform.position;
                        _selectedWare.Pick(this);

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
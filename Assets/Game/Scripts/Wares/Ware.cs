using System.Collections;
using UnityEngine;

public class Ware : MonoBehaviour, IWareSupport
{
    [Header("Settings")] 
    [SerializeField] private LayerMask _obstaclesLayer;
    
    [Header("References")] 
    [SerializeField] private GameObject _highlight;
    [SerializeField] private WareBounds[] _bounds;

    public bool IsPlaced { get; private set; }
    
    private Coroutine _rotationCoroutine;

    public void Initialize()
    {
        IsPlaced = false;
        
        foreach (WareBounds bound in _bounds)
        {
            bound.Initialize(this);
        }
    }

    public void Place(PickManager manager)
    {
        IsPlaced = true;
        SetInteractable(true);
        ClearBoundsIndicators();
    }

    public void Pick(PickManager manager)
    {
        SetInteractable(false);
        transform.parent = manager.transform;
    }
    
    public bool Rotate(int angle, Vector3 offset)
    {
        if (_rotationCoroutine != null)
        {
            return false;
        }

        _rotationCoroutine = StartCoroutine(RotateCoroutine(angle, offset));
        
        return true;
    }
    
    private IEnumerator RotateCoroutine(int angle, Vector3 offset)
    {
        float percent = 0;
        Vector3 initialPosition = transform.position;
        Quaternion initialRotation = transform.rotation;
    
        while (percent < 1)
        {
            percent += Time.deltaTime * 1 / 0.1f;
    
            transform.position = Vector3.Lerp(initialPosition, initialPosition + offset, percent);
            transform.rotation = Quaternion.Lerp(initialRotation, initialRotation * Quaternion.Euler(0, angle, 0), percent);
            
            yield return null;
        }
    
        _rotationCoroutine = null;
    }
    
    public void SetHighlight(bool active)
    {
        _highlight.SetActive(active);
    }

    public void SetInteractable(bool value)
    {
        foreach (WareBounds bound in _bounds)
        {
            bound.SetInteractible(value);
        }
    }
    
    public void UpdateBoundsIndicators()
    {
        foreach (WareBounds bound in _bounds)
        {
            WareBoundsIndicator indicators = 0;

            if (bound.DoesOverlap(_obstaclesLayer))
            {
                indicators |= WareBoundsIndicator.Overlap;
            }
            else
            {
                indicators |= WareBoundsIndicator.Correct;
            }

            // WareBoundsSupport boundsSupport = bound.GetSupport(_obstaclesLayer);
            // if (boundsSupport != WareBoundsSupport.Self && boundsSupport == WareBoundsSupport.None)
            // {
            //     indicators |= WareBoundsIndicator.NoSupport;
            // }
            
            bound.SetIndicator(indicators);
        }
    }

    public void SetBoundsIndicators(WareBoundsIndicator indicator)
    {
        foreach (WareBounds bound in _bounds)
        {
            bound.SetIndicator(indicator);
        }
    }

    public void ClearBoundsIndicators()
    {
        foreach (WareBounds bound in _bounds)
        {
            bound.ClearIndicators();
        }
    }

    public bool CanBePicked(LayerMask wareLayerMask)
    {
        foreach (WareBounds wareBounds in _bounds)
        {
            if (wareBounds.HasWareAbove(wareLayerMask))
            {
                return false;
            }
        }

        return true;
    }
    
    public bool CanBePlaced(LayerMask obstacleLayerMask)
    {
        return DoesOverlapWithObstacle(obstacleLayerMask);
    }
    
    public bool DoesOverlapWithObstacle(LayerMask obstacleLayerMask)
    {
        foreach (WareBounds wareBounds in _bounds)
        {
            if (wareBounds.DoesOverlap(obstacleLayerMask))
            {
                return false;
            }
        }

        return true;
    }

    public bool CanSupportWare(Ware ware, LayerMask supportLayerMask)
    {
        return IsPlaced;
    }

    public Vector3 GetSnapSupportPosition(Ware ware, Vector3 warePosition, Vector3 mouseOffset)
    {
        Vector3 offset = new Vector3(mouseOffset.x, 0, mouseOffset.z);
        return Vector3Int.RoundToInt(warePosition + offset + Vector3.up);
    }
    
#if UNITY_EDITOR
    private void OnValidate()
    {
        _bounds = GetComponentsInChildren<WareBounds>();
    }
#endif
}

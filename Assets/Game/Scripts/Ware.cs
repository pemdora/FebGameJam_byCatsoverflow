using UnityEngine;

public class Ware : MonoBehaviour
{
    [Header("Settings")] 
    [SerializeField] private LayerMask _obstaclesLayer;
    
    [Header("References")] 
    [SerializeField] private GameObject _highlight;
    [SerializeField] private WareBounds[] _bounds;

    // private Coroutine _rotationCoroutine;
    //
    // public bool Rotate(int angle, Vector3 oldOffset)
    // {
    //     if (_rotationCoroutine != null)
    //     {
    //         return false;
    //     }
    //
    //     _rotationCoroutine = StartCoroutine(RotateCoroutine())
    //     
    //     return true;
    // }
    //
    // private IEnumerator RotateCoroutine(Quaternion finalRotation, Vector3 finalPosition)
    // {
    //     float percent = 0;
    //     Quaternion initialRotation = transform.rotation;
    //     Vector3 initialPosition = _rotationPivot.position;
    //
    //     while (percent < 1)
    //     {
    //         percent += Time.deltaTime * 1 / 0.2f;
    //
    //         transform.rotation = Quaternion.Lerp(initialRotation, finalRotation, percent);
    //         transform.position = Vector3.Lerp(initialPosition, finalPosition, percent);
    //         
    //         yield return null;
    //     }
    //
    //     _rotationCoroutine = null;
    // }
    
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
            if (wareBounds.IsSupportingOtherWare(wareLayerMask))
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

    public bool HasEnoughSupport(float percentageMin, LayerMask supportLayerMask)
    {
        int totalSupport = 0;
        int actualSupport = 0;
        
        foreach (WareBounds bound in _bounds)
        {
            switch (bound.GetSupport(supportLayerMask))
            {
                case WareBoundsSupport.None:
                    totalSupport++;
                    break;
                case WareBoundsSupport.Self:
                    break;
                case WareBoundsSupport.CargoSlot:
                case WareBoundsSupport.OtherWare:
                    totalSupport++;
                    actualSupport++;
                    break;
            }
        }

        return (float)actualSupport / totalSupport >= percentageMin;
    }
    
#if UNITY_EDITOR
    private void OnValidate()
    {
        _bounds = GetComponentsInChildren<WareBounds>();
    }
#endif
}

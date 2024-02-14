using System;
using System.Collections;
using UnityEngine;

public class Ware : MonoBehaviour, IWareSupport
{
    [Header("Settings")]
    [SerializeField] private LayerMask _obstaclesLayer;

    [Header("References")]
    [SerializeField] private GameObject _highlight;
    [SerializeField] private WareBounds[] _bounds;
    [SerializeField] private GameObject _graphicObject;
    [SerializeField] private GameObject _graphicObjectContainer;


    private Transform _warePoolContainer;
    public Transform WarePoolContainer { get => _warePoolContainer; }

    private Coroutine _rotationCoroutine;
    private Cargo _associatedCargo;

    public void Initialize(Transform poolTransform)
    {
        _warePoolContainer = poolTransform;

        foreach (WareBounds bound in _bounds)
        {            
            bound.Initialize(this);   
        }  

        if(_graphicObject && _graphicObjectContainer) createGraphicObject();
        
    }

    
    public void createGraphicObject()
    {
        foreach (WareBounds wareBound in _bounds)
        {
            GameObject newGraphicObject = Instantiate(_graphicObject, wareBound.transform.position, Quaternion.identity);
            newGraphicObject.transform.parent = _graphicObjectContainer.transform;
            newGraphicObject.transform.Rotate(-90, 0, 0);
        }
    }
    

    public void Place(Cargo destination)
    {
        _associatedCargo = destination;
        SetInteractable(true);
        ClearBoundsIndicators();
        _associatedCargo.AddWare(this);
        transform.parent = destination.transform;
    }

    public void Pick(PickManager manager)
    {
        if (_associatedCargo != null)
        {
            _associatedCargo.RemoveWare(this);
            _associatedCargo = null;
        }

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
        return _associatedCargo != null;
    }

    public Vector3 GetSnapSupportPosition(Ware ware, Vector3 warePosition, Vector3 mouseOffset)
    {
        Vector3 offset = new Vector3(mouseOffset.x, 0, mouseOffset.z);
        return warePosition + Vector3.up + Vector3Int.RoundToInt(offset);
    }

    public Cargo GetAssociatedCargo()
    {
        return _associatedCargo;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        _bounds = GetComponentsInChildren<WareBounds>();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        foreach (WareBounds wareBounds in _bounds)
        {
            Gizmos.DrawWireCube(wareBounds.transform.position + Vector3.up * 0.5f, Vector3.one);
        }
    }
#endif
}

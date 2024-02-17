using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class Ware : MonoBehaviour, IWareSupport
{

    public enum WareTypes
    {
        Undefined,
        BasicBox,
        CardBox,
        Heavy,
        Explosive
    }


    [Header("Settings")]
    [SerializeField] private LayerMask _obstaclesLayer;
    [SerializeField] private WareTypes _wareType;
    [SerializeField] private AnimationCurve _scaleAnimationCurve; // scale when an object is placed

    [Header("References")]
    [SerializeField] private GameObject _highlight;
    [SerializeField] private WareBounds[] _bounds;
    [SerializeField] private GameObject[] _graphicObject;
    [SerializeField] private GameObject _graphicObjectContainer;

    public Transform WarePoolContainer { get => _warePoolContainer; }


    private GameObject _graphicObjectSelected;
    private int? randomObjectID;  
    private Transform _warePoolContainer;
    private Coroutine _dropCoroutine;
    private Coroutine _rotationCoroutine;
    private Coroutine _scaleCoroutine;
    private float _scaleDuration;
    private WaitForSeconds _waitDropTime = new WaitForSeconds(2.5f);
    private Cargo _associatedCargo;

    void Start()
    {
        if (_wareType == WareTypes.Undefined)
        {
            Debug.LogWarning(gameObject.name + " has no waretype");
        }
    }
    
    void OnEnable()
    {
        RandomizeGraphicObjectSelection();
    }
    
    public void Initialize(Transform poolTransform)
    {
        _warePoolContainer = poolTransform;
       
        foreach (WareBounds bound in _bounds)
        {
            bound.Initialize(this);
            InstantiateGraphicObjectSelectedOnBound(bound);
        }
        _scaleDuration = _scaleAnimationCurve.keys[_scaleAnimationCurve.length - 1].time;
    }

    private void InstantiateGraphicObjectSelectedOnBound(WareBounds bound)
    {
        if (_graphicObject.Length > 0 && _graphicObjectContainer)
        {
            GameObject newGraphicObject = Instantiate(_graphicObjectSelected, bound.transform.position, Quaternion.identity);
            newGraphicObject.transform.parent = _graphicObjectContainer.transform;
        }
    }
    
    public void Place(Cargo destination)
    {
        _associatedCargo = destination;
        SetInteractable(true);
        ClearBoundsIndicators();
        _associatedCargo.AddWare(this);
        transform.parent = destination.transform;
        StartPlaceAnimation();
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

    public void Drop()
    {
        SetInteractable(false);
        ClearBoundsIndicators();
        _associatedCargo = null;
        Fall();
    }

    // make an transform animation to simulate a fall
    private void Fall()
    {
        //Add a rigidbody if there is none
        if (GetComponent<Rigidbody>() == null)
        {
            gameObject.AddComponent<Rigidbody>();
        }
        _dropCoroutine = StartCoroutine(FallCoroutine());
    }

    private IEnumerator FallCoroutine()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;

        yield return _waitDropTime;
        
        AudioManager.Instance.PlayOuch();
        rb.isKinematic = true;
        transform.parent = _warePoolContainer;
        gameObject.SetActive(false);
        _dropCoroutine = null;
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

    private void StartPlaceAnimation()
    {
        if (_scaleCoroutine != null)
        {
            StopCoroutine(_scaleCoroutine);
        }
        if(_scaleAnimationCurve == null || _scaleAnimationCurve.keys.Length == 0)
        {
            Debug.LogWarning("No scale animation curve set for " + name);
            return;
        }
        
        _scaleCoroutine = StartCoroutine(ScaleCoroutine());
    }

    // Scale according to the animation curve
    private IEnumerator ScaleCoroutine()
    {
        float timer = _scaleDuration;
        float ratio = 0;
    
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            ratio = (_scaleDuration - timer) / _scaleDuration;
            float scale = _scaleAnimationCurve.Evaluate(ratio);
            transform.localScale = new Vector3(scale, scale, scale);
            
            yield return null;
        }

        // force scale to last value
        transform.localScale = new Vector3(_scaleAnimationCurve.Evaluate(_scaleDuration), _scaleAnimationCurve.Evaluate(_scaleDuration), _scaleAnimationCurve.Evaluate(_scaleDuration));
    
        _scaleCoroutine = null;
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
    
    private void RandomizeGraphicObjectSelection()
    {
        if (randomObjectID == null && _graphicObject.Length > 0)
        {
            randomObjectID = UnityEngine.Random.Range(0, _graphicObject.Length);
            _graphicObjectSelected = _graphicObject[(int)randomObjectID];
        }
    }

    public WareTypes GetWareType()
    {
        return _wareType;
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

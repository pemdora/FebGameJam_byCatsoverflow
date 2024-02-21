using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Ware : MonoBehaviour, IWareSupport
{
    public enum WareTypes
    {
        Undefined,
        Basic,
        Fragile,
        Livestock,
        Heavy,
        Explosive
    }

    [Header("Settings")]
    [SerializeField] private LayerMask _obstaclesLayer;
    [SerializeField] private WareTypes _wareType;
    [SerializeField] private AnimationCurve _scaleAnimationCurve; // scale when an object is placed
    [SerializeField] private GameObject _UIpointsprefab;

    [Header("References")]
    [SerializeField] private GameObject _graphicsParents;
    [SerializeField] private Material _matOutline;
    [SerializeField] private WareBounds[] _bounds;

    [Header("Events")]
    public UnityEvent OnPick;
    public UnityEvent OnDrop;

    public Transform WarePoolContainer { get => _warePoolContainer; }
    public bool HasBeenPlaced => _associatedCargo != null;
    public int Size => _bounds.Length;

    private List<MeshRenderer> _graphicRenderers = new List<MeshRenderer>();
    private List<Material[]> _originalMaterials = new List<Material[]>(); // is not opti sorry but I suck in shader and stuff
    private List<Material[]> _outlineMaterials = new List<Material[]>();

    private GameObject _graphicObjectSelected;
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

        if (_graphicsParents != null)
        {
            foreach (Transform child in _graphicsParents.transform)
            {
                // Call the method like this:
                AddOutlineToAllMeshRenderers(_graphicsParents.transform, _matOutline);
            }
        }
        else
        {
            Debug.LogWarning("No graphics parent set for " + name);
        }
    }
    
    // Not the best option but it works
    void AddOutlineToAllMeshRenderers(Transform parent, Material matOutline)
    {
        foreach (Transform child in parent)
        {
            MeshRenderer meshRenderer = child.GetComponent<MeshRenderer>();
            if (meshRenderer != null)
            {
                var materials = meshRenderer.materials;
                var newMaterials = new Material[materials.Length + 1];
                materials.CopyTo(newMaterials, 0);
                newMaterials[materials.Length] = matOutline;

                _graphicRenderers.Add(meshRenderer);
                _originalMaterials.Add(materials);
                _outlineMaterials.Add(newMaterials);
            }

            // Recursively call this method for each child
            AddOutlineToAllMeshRenderers(child, matOutline);
        }
    }
    
    public void Initialize(Transform poolTransform)
    {
        _warePoolContainer = poolTransform;

        foreach (WareBounds bound in _bounds)
        {
            bound.Initialize(this);
        }
        _scaleDuration = _scaleAnimationCurve.keys[_scaleAnimationCurve.length - 1].time;
    }

    public void Place(Cargo destination)
    {
        _associatedCargo = destination;
        SetInteractable(true);
        ClearBoundsIndicators();
        _associatedCargo.AddWare(this);
        transform.parent = destination.transform;
        StartPlaceAnimation();
        PointsAnimation();
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

        OnPick?.Invoke();
    }

    public void Drop()
    {
        SetInteractable(false);
        ClearBoundsIndicators();
        _associatedCargo = null;
        Fall();

        OnDrop?.Invoke();
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
        rb.AddTorque(new Vector3(Random.Range(-20, 20), Random.Range(-20, 20), Random.Range(-20, 20)));

        yield return _waitDropTime;

        AudioManager.Instance.PlaySoundEffect(SoundEffectType.OUTCH);
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
        if (_scaleAnimationCurve == null || _scaleAnimationCurve.keys.Length == 0)
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
        if (active)
        {
            for(int i = 0; i < _graphicRenderers.Count; i++)
            {
                _graphicRenderers[i].materials = _outlineMaterials[i];
            }
        }
        else
        {
            for(int i = 0; i < _graphicRenderers.Count; i++)
            {
                _graphicRenderers[i].materials = _originalMaterials[i];
            }
        }
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

    private void PointsAnimation()
    {
        if (!_UIpointsprefab)
        {

        }
        GameObject uiPrefab = Instantiate(_UIpointsprefab, new Vector3(0, 0, 0), Quaternion.identity);
        UIWarePoints warePointsUI = uiPrefab.GetComponent<UIWarePoints>();
        if (warePointsUI)
        {
            // TODO Set the actual score of the ware
            warePointsUI.transform.position = transform.position;
            warePointsUI.SetPointsValue(Random.Range(50, 350));
            warePointsUI.StartAnimation();
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
        if (HasBeenPlaced)
        {
            return false;
        }

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
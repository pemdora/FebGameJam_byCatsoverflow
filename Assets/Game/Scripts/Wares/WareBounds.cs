using System;
using UnityEngine;

[Flags]
public enum WareBoundsIndicator
{
    None,
    Correct,
    Overlap,
    NoSupport,
}

public class WareBounds : MonoBehaviour
{




    [Header("References")] 
    [SerializeField] private Collider _collider;
    [SerializeField] private GameObject _correctIndicator;
    [SerializeField] private GameObject _overlapIndicator;
    [SerializeField] private GameObject _noSupportIndicator;
    
    private Ware _associatedWare;

    public void Initialize(Ware ware)
    {
        _associatedWare = ware;        
    }
    
    public Ware GetWare()
    {
        return _associatedWare;
    }

    public void SetInteractible(bool value)
    {
        _collider.enabled = value;
    }

    public void SetIndicator(WareBoundsIndicator indicator)
    {
        if (indicator.HasFlag(WareBoundsIndicator.None))
        {
            ClearIndicators();
        }
        
        if (indicator.HasFlag(WareBoundsIndicator.Correct))
        {
            _correctIndicator.SetActive(true);
        }
        
        if (indicator.HasFlag(WareBoundsIndicator.Overlap))
        {
            _overlapIndicator.SetActive(true);
        }
        
        if (indicator.HasFlag(WareBoundsIndicator.NoSupport))
        {
            _noSupportIndicator.SetActive(true);
        }
    }

    public void ClearIndicators()
    {
        _correctIndicator.SetActive(false);
        _overlapIndicator.SetActive(false);
        _noSupportIndicator.SetActive(false);
    }

    public bool DoesOverlap(LayerMask obstaclesLayerMask)
    {
        Collider[] hitColliders = Physics.OverlapBox(transform.position + Vector3.up / 2, transform.localScale / (1 / 0.49f), Quaternion.identity, obstaclesLayerMask);

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

    public bool HasWareAbove(LayerMask wareLayerMask)
    {
        Collider[] hitColliders = Physics.OverlapBox(transform.position + Vector3.up * 1.5f, transform.localScale / (1 / 0.49f), Quaternion.identity, wareLayerMask);

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
}
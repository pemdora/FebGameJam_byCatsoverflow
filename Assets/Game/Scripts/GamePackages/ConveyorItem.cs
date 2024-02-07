using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorItem : MonoBehaviour
{
    public Vector3 _direction = Vector3.right;

    public float Speed = 1f;

    private Ware _ware;

    private bool _isActive = false;

    private Vector3 _positionMemory = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        ActivateConveyor();
    }

    // Update is called once per frame
    void Update()
    {
        if (_isActive)
        {
            transform.position += _direction.normalized * Speed * Time.deltaTime;
        }
    }


    public void SetPackage(Ware newPackage)
    {
        if (_ware != null)
        {
            Destroy(_ware);
        }
        _ware = newPackage;
        _ware.transform.parent = transform;
    }

    public void DestroyItem()
    {
        if (_ware)
        {
            Destroy(_ware.gameObject);
        }
        Destroy(gameObject);
    }

    public void ActivateConveyor()
    {
        if (!_isActive)
        {
            _isActive = true;
            transform.position = _positionMemory;
            Collider collider = GetComponentInParent<Collider>();
            if (collider)
            {
                collider.enabled = true;
            }
        }
    }

    public void DeactivateConveyor()
    {
        if (_isActive)
        {
            _isActive = false;
            _positionMemory = transform.position;
            Collider collider = GetComponentInParent<Collider>();
            if (collider)
            {
                collider.enabled = false;
            }
        }
    }



}

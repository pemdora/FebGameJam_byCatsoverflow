using UnityEngine;

public class ConveyorItem : MonoBehaviour
{
    private Transform _transform;
    private ConveyorStart _owner;
    private float _speed;
    private Ware _ware;

    private void Update()
    {
        _transform.position += transform.forward * (_speed * Time.deltaTime);
    }

    public void Initialize(ConveyorStart owner, float speed, Ware ware)
    {
        _transform = transform;
        _owner = owner;
        _speed = speed;
        _ware = ware;

        _ware.transform.parent = _transform;
        _ware.transform.localPosition = Vector3.zero;
        _ware.transform.localRotation = Quaternion.Euler(0, Random.Range(0, 4) * 90, 0);
    }

    public void ReachConveyorEnd()
    {
        _owner.ReturnConveyorItem(this);
    }

    public bool TryGetWare(out Ware ware)
    {
        ware = GetComponentInChildren<Ware>();
        return ware != null;
    }

    public void ClearWare()
    {
        _ware = null;
    }

    public void ChangeSpeed(float speed)
    {
        _speed = speed;
    }
}
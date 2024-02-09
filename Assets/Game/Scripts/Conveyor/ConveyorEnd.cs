using UnityEngine;

public class ConveyorEnd : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out ConveyorItem item))
        {
            item.ReachConveyorEnd();
        }
    }
}
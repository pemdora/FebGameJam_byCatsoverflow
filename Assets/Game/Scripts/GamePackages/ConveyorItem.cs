using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorItem : MonoBehaviour
{
    public Vector3 Direction = Vector3.right;

    public float Speed = 1f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += Direction.normalized * Speed * Time.deltaTime;
    }


    public void SetPackage(Package newPackage)
    {
        if (package != null)
        {
            Destroy(package);
        }
        package = newPackage;
        package.transform.parent = transform;
    }

    public void DestroyItem()
    {
        if (package)
        {
            Destroy(package.gameObject);
        }
        Destroy(gameObject);
    }

    private Package package;

}

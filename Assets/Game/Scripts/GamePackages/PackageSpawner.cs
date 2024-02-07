using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PackageSpawner : Spawner<Ware>
{
    public ConveyorItem conveyorItemPrefab;
    protected override Ware SpawnPackage(Ware GamePackageToSpawn )
    {
        Ware newPackage = base.SpawnPackage(GamePackageToSpawn);
        if ( newPackage != null )
        {
            GameObject newConveyorItem = Instantiate(conveyorItemPrefab.gameObject, transform.position,transform.rotation);
            ConveyorItem currentConveyorItem = newConveyorItem.GetComponent<ConveyorItem>();
            if ( currentConveyorItem != null )
            {
                currentConveyorItem.SetPackage(newPackage);
            }
        }

        return newPackage;

    } 
}

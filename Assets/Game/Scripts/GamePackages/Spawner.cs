using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner<T> : MonoBehaviour where T : MonoBehaviour
{
    [SerializeField]
    protected T[] prefabClasses;

    [SerializeField]
    float TimeBeforeStart = 0.0f;

    [SerializeField]
    float TimeBetweenSpawns = 0.0f;



    int Index = 0;


    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating(nameof(SpawnPackageList), TimeBeforeStart, TimeBetweenSpawns);
    }

    // Update is called once per frame
    void Update()
    {
    }


    // Spawns a Package using GamePackageClass reference 
    void SpawnPackage(T GamePackageToSpawn)
    {
        if (GamePackageToSpawn)
        {
            Vector3 Position = gameObject.transform.position + new Vector3(0, 2.5f, 0);
            GameObject Package = Instantiate(GamePackageToSpawn.gameObject, Position, Quaternion.identity);
            Destroy(Package.gameObject,0.5f) ;
        }
    }

    //Spawn a Package From the list
    void SpawnPackageList()
    {
        T CurrentGamePackageClass = prefabClasses[Index];
        if (CurrentGamePackageClass)
        {
           SpawnPackage(CurrentGamePackageClass);
            Index = (Index + 1) % prefabClasses.Length;
        }
    }

}

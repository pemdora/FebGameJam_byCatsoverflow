using UnityEngine;

public class Spawner<T> : MonoBehaviour where T : MonoBehaviour
{
    [SerializeField] protected T[] prefabClasses;
    [SerializeField] float TimeBeforeStart = 0.0f;
    [SerializeField] float TimeBetweenSpawns = 0.0f;
    
    private int Index = 0;

    void Start()
    {
        InvokeRepeating(nameof(SpawnPackageList), TimeBeforeStart, TimeBetweenSpawns);
    }

    // Spawns a Package using GamePackageClass reference 
    protected virtual T SpawnPackage(T GamePackageToSpawn)
    {
        if (GamePackageToSpawn)
        {
            Vector3 Position = gameObject.transform.position + new Vector3(0, 2.5f, 0);
            GameObject Package = Instantiate(GamePackageToSpawn.gameObject, Position, Quaternion.identity);
            //Destroy(Package.gameObject,0.5f) ;
            return Package.GetComponent<T>();
        }

        return null;
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
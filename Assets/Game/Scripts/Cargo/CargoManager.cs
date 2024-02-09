using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CargoManager : MonoBehaviour
{
    private Cargo _cargo;
    [SerializeField] private LandingPlatform _landingPlatform;
    [SerializeField] private List<Cargo> _cargoPrefabList;
    [SerializeField] private Queue<Cargo> _cargoPrefabQueue;
    // Start is called before the first frame update
    void Start()
    {
        _cargoPrefabQueue = new Queue<Cargo>();
        for (int i = 0; i < _cargoPrefabList.Count; i++)
        {
            _cargoPrefabQueue.Enqueue(_cargoPrefabList[i] );
        }
        if (_landingPlatform)
        {
            _cargo = _landingPlatform.GetComponentInChildren<Cargo>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.I)) {
            NextCargo();
        }
    }

    void NextCargo()
    {
        DestroyCurrentCargo();
        SpawnNextCargo();
    }

    void DestroyCurrentCargo()
    {
        if (_cargo)
        {
            Destroy(_cargo.gameObject);
        }
    }
    void SpawnNextCargo()
    {
        if (_cargoPrefabQueue.Count > 0)
        {
            Cargo cargoPrefab = _cargoPrefabQueue.Dequeue();
            if (cargoPrefab)
            {
                Cargo cargoObject =Instantiate(cargoPrefab);
                _cargo = cargoObject;
                _cargo.transform.SetParent(_landingPlatform.transform, true) ;
            }
        }
    }
}

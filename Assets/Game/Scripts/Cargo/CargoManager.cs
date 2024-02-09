using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CargoManager : MonoBehaviour
{
    private Cargo _cargo;
    [SerializeField] private LandingPlatform _landingPlatform;
    [SerializeField] private List<Cargo> _cargoPrefabList;
    [SerializeField] private Queue<Cargo> _cargoPrefabQueue;
    Coroutine _arrivalCoroutine;
    Coroutine _leaveCoroutine;
    // Start is called before the first frame update
    void Start()
    {
        _cargoPrefabQueue = new Queue<Cargo>();
        for (int i = 0; i < _cargoPrefabList.Count; i++)
        {
            _cargoPrefabQueue.Enqueue(_cargoPrefabList[i]);
        }
        if (_landingPlatform)
        {
            _cargo = _landingPlatform.GetComponentInChildren<Cargo>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.I))
        {
            NextCargo();
        }
    }

    void NextCargo()
    {
        if (_leaveCoroutine  == null && _arrivalCoroutine == null)
        {
        _leaveCoroutine = StartCoroutine(DestroyCargoCoroutine());
        _arrivalCoroutine = StartCoroutine(CargoArrival());
        }
    }


    Cargo SpawnNextCargo()
    {
        if (_cargoPrefabQueue.Count > 0)
        {
            Cargo cargoPrefab = _cargoPrefabQueue.Dequeue();
            if (cargoPrefab)
            {
                Cargo cargoObject = Instantiate(cargoPrefab);
                cargoObject.transform.SetParent(_landingPlatform.transform, true);
                cargoObject.DeactivateCargo();
                return cargoObject;
            }
            return null;

        }
        else
        {
            return null;
        }
    }

    private IEnumerator DestroyCargoCoroutine()
    {
        Cargo cargoToDestroy = _cargo;
        if (cargoToDestroy)
        cargoToDestroy.DeactivateCargo();
        float percent = 0;
        float _duration = 1.0f;
        while (percent < 1)
        {
            percent += Time.deltaTime * 1 / _duration;
            if (cargoToDestroy)
            {
                cargoToDestroy.transform.position += new Vector3(15 * Time.deltaTime, 0, 0);
            }
            else
            {
                //Debug.Log("Became null");
            }
            yield return null;
        }
        if (!cargoToDestroy)
        {
            //Debug.Log("NotDestroy");
        }
        else
        {
            Destroy(cargoToDestroy.gameObject);
        }
        StopCoroutine(_leaveCoroutine);
        _leaveCoroutine = null;
    }

    private IEnumerator CargoArrival()
    {
        Cargo nextCargo = SpawnNextCargo();
        if (nextCargo)
        {
            nextCargo.transform.position -= new Vector3(15, 0, 0);

            float percent = 0;
            float _duration = 1.0f;
            while (percent < 1)
            {
                percent += Time.deltaTime * 1 / _duration;
                nextCargo.transform.position += new Vector3(15 * Time.deltaTime, 0, 0);
                yield return null;

            }
            nextCargo.ActivateCargo();
            _cargo = nextCargo;
            StopCoroutine(_arrivalCoroutine);
            _arrivalCoroutine = null;
        }

    }
}

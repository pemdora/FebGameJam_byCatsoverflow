using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FlyLightArray : MonoBehaviour
{

       
    private List<GameObject> lights = new List<GameObject>();   
    private List<Material> _materials = new List<Material>();
    

    [Header("Settings")]
    [SerializeField] private int _numberOfItems;
    [SerializeField] private float _spacing;
    [SerializeField] private int _emissionPosition = 0;
    [SerializeField] private Vector3 axis;  
    [SerializeField] private Vector3 _rotation;
    [SerializeField] private Color _onEmissionColor;
    [SerializeField] private Color _offEmissionColor = Color.black;
      
    
    [Header("Reference")]
    [SerializeField] private GameObject _objet;
    [SerializeField] private GameObject _objectContainer; 
    

    void Start()
    {
        if(!_objectContainer) throw new MissingComponentException("missing object container");        
        if(!_objet) throw new MissingComponentException("missing object to duplicate");    
        if(_objet.GetComponent<MeshRenderer>() == null) throw new MissingComponentException("missing mesh renderer on object to duplicate");
        if(axis.x != 0 && axis.x != 1 || axis.y != 0 && axis.y != 1 || axis.z != 0 && axis.z != 1) throw new MissingComponentException("axis must be 0 or 1");
        if(_onEmissionColor == null) throw new MissingComponentException("missing on emission color");

        _materials = _objet.GetComponent<MeshRenderer>().sharedMaterials.ToList();    
        GenerateLights(_numberOfItems);
        StartCoroutine(LightSequences());
    }



    IEnumerator LightSequences()
    {
        while (true)
        {
            for (int i = 0; i < lights.Count; i++)
            {
                // lights[i].GetComponent<MeshRenderer>().sharedMaterials[_emissionPosition].color =  _onEmissionColor; //à utiliser si utilisation de shader
                lights[i].GetComponent<MeshRenderer>().material.EnableKeyword("_EMISSION");
                lights[i].GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", _onEmissionColor);
                yield return new WaitForSeconds(0.1f);
            }
            for (int i = 0; i < lights.Count; i++)
            {   
                lights[i].GetComponent<MeshRenderer>().material.DisableKeyword("_EMISSION");
                // lights[i].GetComponent<MeshRenderer>().sharedMaterials[_emissionPosition].color =  _offEmissionColor; à utiliser si utilisation de shader
                lights[i].GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", _offEmissionColor);
                Debug.Log(_offEmissionColor);
                yield return new WaitForSeconds(0.1f);
            }
        }
    }



    void GenerateLights(int numberOfItems)
    {
        for (int i = 0; i < numberOfItems; i++)
        {
            Vector3 position = this.gameObject.transform.position + i * _spacing * axis.normalized; 
            GameObject lightObject = Instantiate(_objet, position, Quaternion.identity); 
            lightObject.transform.parent= _objectContainer.transform;
            lightObject.transform.Rotate(_rotation); //correction de la rotation,  surement du à un problème de pivot sur le prefab
            lights.Add(lightObject);
        }
    }
    void Update()
    {
        
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.UIElements;
using UnityEngine;

public class FlyLightArray : MonoBehaviour
{

       
    private List<GameObject> lights = new List<GameObject>();   
    private List<Material> _materials = new List<Material>();
    

    [Header("Settings")]
    [SerializeField] private int _numberOfItems;
    [SerializeField] private float _spacing;
    [SerializeField] private int? _emissionPosition = 0;
    [SerializeField] private Vector3 axis;  
    [SerializeField] private Vector3 _rotation;

    [ColorUsageAttribute(true, true)]
    [SerializeField] private Color _onEmissionColor;

    [ColorUsageAttribute(true, true)]
    [SerializeField] private Color _offEmissionColor = Color.black;

    [Header("Animation")]
    [Range(0, 2)]
    [SerializeField] private float _BaseLightDuration;

    //curve animation
    [SerializeField] private AnimationCurve _lightAnimationCurve;

    [Header("Warning Animation")]
    [ColorUsageAttribute(true, true)]
    [SerializeField] private Color _warningColor;
    [SerializeField] private float _timeBeforeWarning;   
    [Range(0, 2)]    
    [SerializeField] private float _WarningLightDuration;

    private float _lightDuration;  
    private Color _currentEmissionColor;
    private Color _currentWarningColor;
    
    [Header("Reference")]
    [SerializeField] private GameObject _objet;
    [SerializeField] private GameObject _objectContainer; 
    [SerializeField] private Light _realLight;
    [SerializeField] private SpaceshipManager _spaceShipManager;
    

    void Start()
    {
        checkSettingAndRef();           
        _realLight.color = _onEmissionColor; //set the color of the real light to the color of the emission
        _lightDuration = _BaseLightDuration;
        _currentEmissionColor = _onEmissionColor;
        _currentWarningColor = _warningColor;
        GenerateLights(_numberOfItems);
        StartCoroutine(LightSequences());
    }

    void Update()
    {
        if (_spaceShipManager.IsAvailable && _spaceShipManager.TimeRemaining<_timeBeforeWarning) {
            ChangeLightDuration(_WarningLightDuration);        
            ChangeEmissionColor(_warningColor);
        }else{
            
            ChangeLightDuration(_BaseLightDuration);       
            ChangeEmissionColor(_onEmissionColor);   
        }
    }


        IEnumerator LightSequences()
    {
        while (true)
        {
            for (int i = 0; i < lights.Count; i++)
            {        
                turnLightOn(i);        
                _realLight.transform.position = lights[i].transform.position; //displace real light to the position of the light

                //variation du temps entre chaque lumière en fonction de la courbe
                float timer = _lightDuration * _lightAnimationCurve.Evaluate((float)i / (float)lights.Count);
                yield return new WaitForSeconds(timer);
                

                turnLightOff(i);
            
            }
        }
    }



    private void turnLightOn(int index)
    {
        // lights[i].GetComponent<MeshRenderer>().sharedMaterials[_emissionPosition].color =  _onEmissionColor; //à utiliser si utilisation de shader
        lights[index].GetComponent<MeshRenderer>().material.EnableKeyword("_EMISSION");
        lights[index].GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", _onEmissionColor);
    }

    private void turnLightOff(int index)
    {
        // lights[i].GetComponent<MeshRenderer>().sharedMaterials[_emissionPosition].color =  _offEmissionColor; //à utiliser si utilisation de shader
        lights[index].GetComponent<MeshRenderer>().material.DisableKeyword("_EMISSION");
        lights[index].GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", _offEmissionColor);
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
    
    public void ChangeEmissionColor(Color color)
    {
        _currentEmissionColor = color;
        _realLight.color = _currentEmissionColor;
    }


    public void ChangeLightDuration(float duration)
    {
        _lightDuration = duration;
    }




    private void checkSettingAndRef()
    {
        
        if (!_objectContainer) throw new MissingComponentException("missing object container");
        if (!_objet) throw new MissingComponentException("missing object to duplicate");
        if (_objet.GetComponent<MeshRenderer>() == null) throw new MissingComponentException("missing mesh renderer on object to duplicate");
        if (axis.x != 0 && axis.x != 1 || axis.y != 0 && axis.y != 1 || axis.z != 0 && axis.z != 1) throw new MissingComponentException("axis must be 0 or 1");
        if (_onEmissionColor == null) throw new MissingComponentException("missing on emission color");
        if (!_realLight) throw new MissingComponentException("missing light prefab");
    }
}

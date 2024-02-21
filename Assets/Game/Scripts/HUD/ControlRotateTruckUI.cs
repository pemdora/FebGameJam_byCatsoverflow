using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Unity.VisualScripting;


public class ControlRotateTruckUI : MonoBehaviour
{
    SpaceshipManager _SpaceshipManager;
    float _transparency;
    bool _landed = false;

    [SerializeField] List<Image> _buttons;
    [SerializeField] float _fadeSpeed;

    public void Start()
    {
        _SpaceshipManager = FindObjectOfType<SpaceshipManager>();
        _SpaceshipManager.OnSpaceshipLanded.AddListener(OnSpaceShipLand);
        _SpaceshipManager.OnSpaceshipTakeOff.AddListener(OnSpaceShipTakeoff);
    }

    public void Update()
    {
        if (_landed)
        {
            _transparency += Time.deltaTime * _fadeSpeed;

        }
        else
        {
            _transparency -= Time.deltaTime * _fadeSpeed;
        }
        _transparency = Mathf.Clamp01(_transparency);

        foreach(var btn in _buttons)
        {
            btn.color = new Color(1, 1, 1, _transparency);
        }
    }

    public void OnSpaceShipLand(Spaceship ship)
    {
        _landed = true;
    }

    public void OnSpaceShipTakeoff(Spaceship ship)
    {
        _landed = false;
    }
}
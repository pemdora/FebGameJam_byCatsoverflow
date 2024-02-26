using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ControlRotateTruckUI : MonoBehaviour
{
    SpaceshipManager _SpaceshipManager;
    LandingPlatform _landingPlatform;
    float _transparency;
    bool _landed = false;
    bool _hasRotate;

    [SerializeField] List<Image> _buttons;
    [SerializeField] float _fadeSpeed;

    public void Start()
    {
        _SpaceshipManager = FindObjectOfType<SpaceshipManager>();
        _SpaceshipManager.OnSpaceshipLanded.AddListener(OnSpaceShipLand);
        _SpaceshipManager.OnSpaceshipTakeOff.AddListener(OnSpaceShipTakeoff);
        _landingPlatform = FindObjectOfType<LandingPlatform>();
        _landingPlatform.OnRotate.AddListener(OnLandingPlatformRotate);
        FindObjectOfType<GameManager>().OnGameOverEvent.AddListener(OnGameOver);
    }

    public void Update()
    {
        if (_landed && !_hasRotate)
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

    private void OnLandingPlatformRotate()
    {
        _hasRotate = true;
    }
    
    private void OnGameOver()
    {
        _hasRotate = false;
    }
}
using System;
using UnityEngine;
using UnityEngine.UI;

public class FrustrationUI : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float _updateDelay = 0.6f;
    [SerializeField] private float _fillSpeed = 1;
    
    [Header("References")]
    [SerializeField] private Image _filler;
    [SerializeField] private Image _fillerBackground;

    private bool _shouldUpdate;
    private float _updateDelayLeft;
    private float _fillAcceleration;
    
    private void Awake()
    {
        _filler.fillAmount = 0f;
        _fillerBackground.fillAmount = 0f;
    }

    private void Update()
    {
        if (_shouldUpdate)
        {
            if (_updateDelayLeft > 0)
            {
                _updateDelayLeft -= Time.deltaTime;
                return;
            }

            _fillAcceleration += Time.deltaTime * 5;
            _filler.fillAmount = _filler.fillAmount + _fillSpeed * _fillAcceleration * Time.deltaTime;
            if (_filler.fillAmount >= _fillerBackground.fillAmount)
            {
                _filler.fillAmount = _fillerBackground.fillAmount;
                _shouldUpdate = false;
            }
        }
    }

    public void UpdateFiller(float value)
    {
        if (!_shouldUpdate)
        {
            _shouldUpdate = true;
            _updateDelayLeft = _updateDelay;
            _fillAcceleration = 0;
        }

        _fillerBackground.fillAmount = Mathf.Clamp01(value);
    }
}
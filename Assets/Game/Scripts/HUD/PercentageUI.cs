using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PercentageUI : MonoBehaviour
{
    [SerializeField] private SpaceshipManager _spaceshipManager;
    [SerializeField] private TMP_Text _percentageText;
    [SerializeField] private Image _filler;
    private float _previousPercentage;
    // Start is called before the first frame update
    void Start()
    {
        _percentageText.text = "??";
        _previousPercentage = 0;
        _filler.fillAmount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (_spaceshipManager.HasSpaceship && _spaceshipManager.Percentage<=100) {
            _previousPercentage = Mathf.FloorToInt(_spaceshipManager.Percentage);
            _percentageText.text = (_previousPercentage).ToString();
            _filler.fillAmount = _spaceshipManager.Percentage / 100f;
        }else {
            _previousPercentage = 0;
            _percentageText.text = "0";
            _filler.fillAmount = 0;
        }
    }
}

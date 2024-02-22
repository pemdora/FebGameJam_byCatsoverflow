using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PercentageUI : MonoBehaviour
{
    [SerializeField] private SpaceshipManager _spaceshipManager;
    [SerializeField] private TMP_Text _percentageText;
    [SerializeField] private Slider _objectiveSlider;
    [SerializeField] private Image _filler;
    [SerializeField] private float _animationDuration = 1.0f;
    private float _previousPercentage;
    private float _animationPercentage;
    private Coroutine _fillCoroutine;

    void Start()
    {
        _percentageText.text = "??";
        _previousPercentage = 0;
        _filler.fillAmount = 0;
    }

    public void SetObjectiveSlider(int frustrationThreshold)
    {
        _objectiveSlider.value = (100 - frustrationThreshold) / 100f;
    }

    void Update()
    {
        if (_spaceshipManager == null)
        {
            return;
        }

        if (_spaceshipManager.HasSpaceship && _spaceshipManager.Percentage <= 100)
        {
                int nextPercentage = Mathf.FloorToInt(_spaceshipManager.Percentage);
            if (nextPercentage != _previousPercentage)
            {
                if (_fillCoroutine == null)
                {
                    _fillCoroutine = StartCoroutine(FillCoroutine(_previousPercentage,nextPercentage,_animationDuration));
                }
                else
                {
                    StopCoroutine(_fillCoroutine);
                    _fillCoroutine = StartCoroutine(FillCoroutine(_animationPercentage, nextPercentage, _animationDuration));
                }
                _previousPercentage = nextPercentage;
            }
        }
        else
        {
            _previousPercentage = 0;
            _percentageText.text = "0";
            _filler.fillAmount = 0;
        }
    }

    private IEnumerator FillCoroutine(float previousValue, float NextValue, float duration)
    {
        float percent = 0;
        while (percent < 1)
        {
            percent += Time.deltaTime / duration;
            _animationPercentage = Mathf.FloorToInt(Mathf.Lerp(previousValue, NextValue, percent));
            Debug.Log("LerpVal = " + _animationPercentage.ToString() +", percent = " +percent);
            _percentageText.text = _animationPercentage.ToString();
            _filler.fillAmount = _animationPercentage / 100f;
            yield return null;
        }
        EndFillCoroutine();
    }

    private void EndFillCoroutine()
    {
        _fillCoroutine = null;
    }
}

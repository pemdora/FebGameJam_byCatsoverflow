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
    [SerializeField] private Image _handleImage;
    [SerializeField] private float _animationDuration = 1.0f;
    [SerializeField] private float _handleAnimationMaxDuration = 1.0f;
    [SerializeField] private Color _handleNormalColor;
    [SerializeField] private Color _handleCompleteColor;



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
        float oldvalue = _objectiveSlider.value;
        _objectiveSlider.value = (100 - frustrationThreshold) / 100f;
        float duration = Mathf.Abs(oldvalue - _objectiveSlider.value) / _handleAnimationMaxDuration;
        StartCoroutine(ObjectiveSliderCoroutine(oldvalue, _objectiveSlider.value, duration));
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
                int percentageDifference = Mathf.FloorToInt(nextPercentage - _previousPercentage);
                float animationTime = (percentageDifference > 3) ? _animationDuration : 0;
                if (_fillCoroutine == null)
                {
                    _fillCoroutine = StartCoroutine(FillCoroutine(_previousPercentage, nextPercentage, animationTime));
                }
                else
                {
                    StopCoroutine(_fillCoroutine);
                    _fillCoroutine = StartCoroutine(FillCoroutine(_animationPercentage, nextPercentage, animationTime));
                }
                _previousPercentage = nextPercentage;
            }
        }
        else
        {
            ResetFilling();
        }
    }

    private IEnumerator FillCoroutine(float previousValue, float NextValue, float duration)
    {
        float percent = 0;
        while (percent < 1)
        {
            percent += Time.deltaTime / duration;
            _animationPercentage = Mathf.FloorToInt(Mathf.Lerp(previousValue, NextValue, percent));
            _percentageText.text = _animationPercentage.ToString();
            _filler.fillAmount = _animationPercentage / 100f;
            if (_filler.fillAmount >= _objectiveSlider.value)
            {
                _handleImage.CrossFadeColor(_handleCompleteColor, 0.25f, false, false);
            }
            yield return null;
        }
        EndFillCoroutine();
    }

    private void EndFillCoroutine()
    {
        _fillCoroutine = null;
    }

    public void ResetFilling()
    {
        if (_fillCoroutine != null)
        {
            StopCoroutine(_fillCoroutine);
        }
        _fillCoroutine = null;
        _previousPercentage = 0;
        _percentageText.text = "0";
        _animationPercentage = 0;
        _filler.fillAmount = 0;
        _handleImage.CrossFadeColor(_handleNormalColor, 0.25f, false, false);

    }

    private IEnumerator ObjectiveSliderCoroutine(float previousValue, float NextValue, float duration)
    {
        float percent = 0;
        
        while (percent < 1)
        {
            percent += Time.deltaTime / duration;
            float sliderValue = Mathf.Lerp(previousValue, NextValue, percent);
            _objectiveSlider.value = sliderValue;
            yield return null;
        }
    }
}

using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PercentageUI : MonoBehaviour
{

    [Header("Settings")]
    [SerializeField] private float _animationDuration = 1.0f;
    [SerializeField] private float _handleAnimationMaxDuration = 1.0f;
    [SerializeField] private float _warningFlickeringSpeed = 0.4f;
    [SerializeField] private Color _handleNormalColor;
    [SerializeField] private Color _handleCompleteColor;
    [SerializeField] private Color _handleWarningColor;

    [Header("References")]
    [SerializeField] private SpaceshipManager _spaceshipManager;
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private TMP_Text _percentageText;
    [SerializeField] private Slider _objectiveSlider;
    [SerializeField] private Image _filler;
    [SerializeField] private RectTransform _fillerTransform;
    [SerializeField] private Image _tresholdGap;
    [SerializeField] private RectTransform _tresholdGapTransform;
    [SerializeField] private Image _handleImage;




    private float _previousPercentage;
    private float _animationPercentage;
    private Coroutine _fillCoroutine;
    private Coroutine _warningCoroutine;
    bool _warningFlickeringColor = true;
    bool _objectiveReached = false;


    void Start()
    {
        FindObjectOfType<ScoreManager>().OnScoreChanged.AddListener(OnScoreChanged);
        _percentageText.text = "??";
        _previousPercentage = 0;
        _filler.fillAmount = 0;
        _tresholdGap.fillAmount = 0;
    }
    public void OnScoreChanged(string lolpourquoiunstring)
    {
        SetGapPositionAndDimensions();
    }
    public void SetGapPositionAndDimensions()
    {
        var pos = new Vector2(-225, _tresholdGapTransform.position.y);
        float maxWidth = _fillerTransform.sizeDelta.x;
        pos.x += maxWidth * _spaceshipManager.Percentage;
        
        float width = _objectiveSlider.value * maxWidth - _spaceshipManager.Percentage * maxWidth;
        _fillerTransform.sizeDelta = new Vector2(width, _fillerTransform.sizeDelta.y);
    }

    //Not used anywhere ?
    public void SetObjectiveSlider(int frustrationThreshold)
    {
        float oldvalue = _objectiveSlider.value;
        _objectiveSlider.value = (frustrationThreshold / 100f);
        float duration = Mathf.Abs(oldvalue - _objectiveSlider.value) / _handleAnimationMaxDuration;
        StartCoroutine(ObjectiveSliderCoroutine(oldvalue, _objectiveSlider.value, duration));
    }

    void Update()
    {
        if (_spaceshipManager == null)
        {
            return;
        }
        if (!_objectiveReached && _warningCoroutine == null && _spaceshipManager.HasSpaceship && _spaceshipManager.TimeRemaining < _gameManager.TimeBeforeWarning)
        {
            _warningFlickeringColor = true;
            _warningCoroutine = StartCoroutine(WarningHandleCoroutine(_warningFlickeringSpeed));
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

    void ObjectiveReached()
    {
        if (_warningCoroutine != null)
        {
            StopCoroutine(_warningCoroutine);
        }
        _warningCoroutine = null;
        _objectiveReached = true;
        _handleImage.CrossFadeColor(_handleCompleteColor, 0.25f, false, false);
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
                ObjectiveReached();
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
        if (_warningCoroutine != null)
        {
            StopCoroutine(_warningCoroutine);
        }
        _warningCoroutine = null;

        _previousPercentage = 0;
        _percentageText.text = "0";
        _animationPercentage = 0;
        _filler.fillAmount = 0;
        _objectiveReached = false;
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

    private IEnumerator WarningHandleCoroutine(float flickeringTime)
    {
        while (_spaceshipManager.HasSpaceship)
        {
            Color color = _handleNormalColor;
            if (_warningFlickeringColor)
            {
                color = _handleWarningColor;
            }
            _warningFlickeringColor = !_warningFlickeringColor;
            _handleImage.CrossFadeColor(color, flickeringTime, false, false);
            yield return new WaitForSeconds(flickeringTime);
        }
    }




}

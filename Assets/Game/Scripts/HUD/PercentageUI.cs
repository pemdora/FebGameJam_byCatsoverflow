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
    [SerializeField] private Image _fillerJuice;
    [SerializeField] private RectTransform _fillerTransform;
    [SerializeField] private Image _tresholdGap;
    [SerializeField] private RectTransform _tresholdGapTransform;
    [SerializeField] private Image _handleImage;

    [SerializeField] private RectTransform _angryStart;
    [SerializeField] private Image _frustrationFill;
    [SerializeField] private RectTransform _frustrationFillTransform;
    [SerializeField] private RectTransform _frustrationBottom;


    private float _previousPercentage;
    private float _animationPercentage;
    private Coroutine _fillCoroutine;
    private Coroutine _warningCoroutine;
    private Coroutine _gapCoroutine = null;
    bool _warningFlickeringColor = true;
    bool _objectiveReached = false;

    void Start()
    {
        _percentageText.text = "0";
        _previousPercentage = 0;
        _filler.fillAmount = 0;
        _tresholdGap.fillAmount = 0;
        _fillerJuice.fillAmount = 0;
        _spaceshipManager.OnSpaceshipTakeOff.AddListener(OnSpaceshipLeft);
        SetGapPositionAndDimensions();
    }
    public void OnSpaceshipLeft(Spaceship spaceship)
    {
        _tresholdGap.fillAmount = 1;
        _fillerJuice.fillAmount = 0;
        SetGapPositionAndDimensions();
        if (_gapCoroutine == null)
            _gapCoroutine = StartCoroutine(GapCoroutine());

        //Set angry emote position start and end



    }
    public void SetGapPositionAndDimensions()
    {
        var pos = new Vector2(-225, _tresholdGapTransform.localPosition.y);
        float maxWidth = _fillerTransform.sizeDelta.x;
        pos.x += maxWidth * _spaceshipManager.Percentage / 100;

        float width = Mathf.Clamp(_objectiveSlider.value * maxWidth - _spaceshipManager.Percentage / 100 * maxWidth, 0, maxWidth);
        _tresholdGapTransform.sizeDelta = new Vector2(width, _fillerTransform.sizeDelta.y);
        _tresholdGapTransform.localPosition = pos;
        _angryStart.position = _tresholdGapTransform.position;
    }

    public void SetObjectiveSlider(int frustrationThreshold)
    {
        float oldvalue = _objectiveSlider.value;
        _objectiveSlider.value = (frustrationThreshold / 100f);
        float duration = Mathf.Abs(oldvalue - _objectiveSlider.value) / _handleAnimationMaxDuration;
        StartCoroutine(ObjectiveSliderCoroutine(oldvalue, _objectiveSlider.value, duration));
        ResetFilling();
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

        /*if (_spaceshipManager.Percentage <= 100)
        {*/
        //Each time the loading percentage changes
        if (_spaceshipManager.HasSpaceship)
        {
            if (Mathf.Abs(_spaceshipManager.Percentage - _previousPercentage) > 0.001f)
            {
                SetGapPositionAndDimensions();

                _fillerJuice.fillAmount = _spaceshipManager.Percentage / 100;

                if (_fillCoroutine == null)
                {
                    _fillCoroutine = StartCoroutine(FillCoroutine(_previousPercentage, _spaceshipManager.Percentage, _animationDuration));
                }
                else
                {
                    StopCoroutine(_fillCoroutine);
                    _fillCoroutine = StartCoroutine(FillCoroutine(_animationPercentage, _spaceshipManager.Percentage, _animationDuration));
                }
                _previousPercentage = _spaceshipManager.Percentage;
            }
        }
        // }
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
    private IEnumerator GapCoroutine()
    {
        while (_tresholdGap.fillAmount > 0)
        {
            _tresholdGap.fillAmount -= Time.deltaTime * 1f;
            yield return null;
        }
        _gapCoroutine = null;
    }
    private IEnumerator FillCoroutine(float previousValue, float NextValue, float duration)
    {
        float percent = 0;
        while (percent < 1)
        {
            percent += Time.deltaTime / duration;
            _animationPercentage = Mathf.Lerp(previousValue, NextValue, percent);
            _percentageText.text = _animationPercentage.ToString("0");
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

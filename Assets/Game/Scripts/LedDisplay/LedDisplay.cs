using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class LedDisplay : MonoBehaviour
{
    [Header("Settings")] 
    [SerializeField] private LedDisplaySettings _settings;
    [SerializeField] private float _spaceByLetters = 120;
    [SerializeField] private float _offset = 45;
    
    [Header("References")] 
    [SerializeField] private TMP_Text _text;
    [SerializeField] private RectTransform _rect;
    [SerializeField] private RectTransform _canvas;
    [SerializeField] private GameObject _glitch1;
    [SerializeField] private GameObject _glitch2;
    [SerializeField] private PickManager _pickManager;
    [SerializeField] private ScoreManager _scoreManager;

    private bool _isDisplaying;
    private float _maxPos;
    private int _previousCommonSentenceIndex;
    private LedDisplayMessageType _previousType;
    
    private bool _displayGlitch;
    private float _glitchDuration;

    private void OnEnable()
    {
        _pickManager.OnDropWare.AddListener(DisplayDiscardSentence);
        _scoreManager.OnCargoReachedMinimumRequirement.AddListener(DisplayCargoSentence);
    }

    private void OnDisable()
    {
        _pickManager.OnDropWare.RemoveListener(DisplayDiscardSentence);
        _scoreManager.OnCargoReachedMinimumRequirement.RemoveListener(DisplayCargoSentence);
    }

    private void Start()
    {
        switch (_settings.order)
        {
            case LedDisplayOrder.Sequenced:
                DisplaySentence(LedDisplayMessageType.Common, 0);
                break;
            case LedDisplayOrder.Randomized:
                DisplaySentence(LedDisplayMessageType.Common, Random.Range(0, _settings.commonSentences.Length));
                break;
        }
    }

    private void Update()
    {
        if (_displayGlitch)
        {
            if (_glitch1.activeSelf && _glitchDuration > _settings.glitchDuration / 2)
            {
                _glitch1.SetActive(false);
                _glitch2.SetActive(true);
            }
            
            if (_glitchDuration >= _settings.glitchDuration)
            {
                _displayGlitch = false;
                _glitch1.SetActive(false);
                _glitch2.SetActive(false);
            }

            _glitchDuration += Time.deltaTime;
            return;
        }
        
        if (_isDisplaying)
        {
            _rect.anchoredPosition += Vector2.left * Time.deltaTime * _settings.speed;

            if (_rect.anchoredPosition.x <= -_maxPos)
            {
                // Reached end
                _isDisplaying = false;

                switch (_settings.order)
                {
                    case LedDisplayOrder.Sequenced:
                        _previousCommonSentenceIndex++;
                        if (_previousCommonSentenceIndex >= _settings.commonSentences.Length)
                        {
                            _previousCommonSentenceIndex = 0;
                        }
                        break;
                    case LedDisplayOrder.Randomized:
                        int nextSentence = _previousCommonSentenceIndex;
                        while (nextSentence == _previousCommonSentenceIndex)
                        {
                            nextSentence = Random.Range(0, _settings.commonSentences.Length);
                        }
                        _previousCommonSentenceIndex = nextSentence;
                        break;
                }
                
                DisplaySentence(LedDisplayMessageType.Common, _previousCommonSentenceIndex);
            }
        }
    }

    public void DisplaySentence(LedDisplayMessageType type, int sentenceID = -1)
    {
        if (type != LedDisplayMessageType.Common && type == _previousType)
        {
            return;
        }
        
        if (_isDisplaying)
        {
            DisplayGlitch();
        }

        _previousType = type;
        if (type == LedDisplayMessageType.Common)
        {
            _previousCommonSentenceIndex = sentenceID;
        }

        if (sentenceID == -1)
        {
            sentenceID = _settings.GetRandomSentenceIndex(type);
        }

        _isDisplaying = true;
        
        _text.text = _settings.GetSentence(type, sentenceID);
        _text.color = _settings.GetColor(type);
        _maxPos = _text.text.Length * _spaceByLetters + _offset + _canvas.sizeDelta.x;
        Move(0);
    }

    private void DisplayCargoSentence(bool success)
    {
        DisplaySentence(success ? LedDisplayMessageType.Success : LedDisplayMessageType.Failure);
    }
    
    private void DisplayDiscardSentence(WareEventData ware)
    {
        DisplaySentence(LedDisplayMessageType.Discard);
    }

    private void DisplayGlitch()
    {
        _displayGlitch = true;
        _glitchDuration = 0;
        _glitch1.SetActive(true);
    }

    private void Move(float position)
    {
        Vector3 pos = _rect.anchoredPosition;
        pos.x = _offset - position;
        _rect.anchoredPosition = pos;
    }
}
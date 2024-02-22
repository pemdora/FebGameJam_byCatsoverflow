using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LedDisplay : MonoBehaviour
{
    [Header("Settings")] 
    [SerializeField] private float _spaceByLetters = 120;
    [SerializeField] private float _offset = 45;
    [SerializeField] private float _speed = 10;
    [SerializeField] private List<string> _sentences;
    
    [Header("References")] 
    [SerializeField] private TMP_Text _text;
    [SerializeField] private RectTransform _rect;
    [SerializeField] private RectTransform _canvas;

    private bool _isDisplaying;
    private float _maxPos;
    private int _sentenceIndex;

    private void Start()
    {
        DisplaySentence(0);
    }

    private void Update()
    {
        if (_isDisplaying)
        {
            _rect.anchoredPosition += Vector2.left * Time.deltaTime * _speed;

            if (_rect.anchoredPosition.x <= -_maxPos)
            {
                // Reached end
                _isDisplaying = false;

                _sentenceIndex++;
                if (_sentenceIndex >= _sentences.Count)
                {
                    _sentenceIndex = 0;
                }
                DisplaySentence(_sentenceIndex);
            }
        }
    }

    public void DisplaySentence(int sentenceID)
    {
        if (_isDisplaying)
        {
            Debug.LogWarning("Cannot display sentence as the previous one has not finish yet.");
            return;
        }
        
        _isDisplaying = true;
        _text.text = _sentences[sentenceID];
        _maxPos = _text.text.Length * _spaceByLetters + _offset + _canvas.sizeDelta.x;
        Move(0);
    }

    private void Move(float position)
    {
        Vector3 pos = _rect.anchoredPosition;
        pos.x = _offset - position;
        _rect.anchoredPosition = pos;
    }
}
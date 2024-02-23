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

    private bool _isDisplaying;
    private float _maxPos;
    private int _sentenceIndex;
    
    private bool _displayGlitch;
    private float _glitchDuration;

    private void Start()
    {
        switch (_settings.order)
        {
            case LedDisplayOrder.Sequenced:
                DisplaySentence(0);
                break;
            case LedDisplayOrder.Randomized:
                DisplaySentence(Random.Range(0, _settings.sentences.Length));
                break;
        }
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.U))
        {
            DisplaySentence(2);
        }
        
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
                //DisplaySentence(_sentenceIndex);
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
                        _sentenceIndex++;
                        if (_sentenceIndex >= _settings.sentences.Length)
                        {
                            _sentenceIndex = 0;
                        }
                        break;
                    case LedDisplayOrder.Randomized:
                        int nextSentence = _sentenceIndex;
                        while (nextSentence == _sentenceIndex)
                        {
                            nextSentence = Random.Range(0, _settings.sentences.Length);
                        }
                        _sentenceIndex = nextSentence;
                        break;
                }
                
                DisplaySentence(_sentenceIndex);
            }
        }
    }

    public void DisplaySentence(int sentenceID)
    {
        if (_isDisplaying)
        {
            DisplayGlitch();
        }

        _sentenceIndex = sentenceID;
        _isDisplaying = true;
        _text.text = _settings.sentences[sentenceID];
        _maxPos = _text.text.Length * _spaceByLetters + _offset + _canvas.sizeDelta.x;
        Move(0);
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
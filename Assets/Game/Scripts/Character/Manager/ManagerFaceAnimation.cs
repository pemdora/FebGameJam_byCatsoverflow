using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;


[System.Serializable]
public class FaceAnimation
{
    public string name;
    public Sprite texture;
}

//emission const

public enum ManagerEmotions
{
    Neutral,
    Pleased,
    Happy,
    Unhappy,
    Livid
}
[System.Serializable]
struct EmotionValues
{
    public int _eyes;
    public int _mouth;
}

[System.Serializable]
struct EmotionDictionaryEntry
{
    public ManagerEmotions _emotion;
    public EmotionValues _values;
}



public class ManagerFaceAnimation : MonoBehaviour
{

    private string EMISSION = "_Emission";
    private string COLOR = "_Color";
    private string BASE_MAP = "Base_Map";

    [Header("Animations settings")]

    [SerializeField] private Color _color = Color.white;
    //hdr color emission
    [ColorUsageAttribute(true, true)]
    [SerializeField] private Color _emissionColor = Color.white;
    // List of the emotions that can make the manager, associed to the mouth and eyes index
    [SerializeField] private List<EmotionDictionaryEntry> _emotionDictionary = new List<EmotionDictionaryEntry>();
    [SerializeField] ManagerEmotions _currentEmotion = ManagerEmotions.Neutral;
    [SerializeField] Vector3 _eyeOffset;
    [SerializeField] Gradient _gradient;


    [Header("Face Animations")]
    [SerializeField] private List<FaceAnimation> eyesAnimations = new List<FaceAnimation>();
    [SerializeField] private List<FaceAnimation> mouthAnimations = new List<FaceAnimation>();


    [Header("références")]
    [SerializeField] private DecalProjector eyesDecalProjector;
    [SerializeField] private DecalProjector mouthDecalProjector;
    [SerializeField] private Material eyeDecalMaterial;
    [SerializeField] private Material mouthDecalMaterial;
    [SerializeField] private GameObject face;
    [SerializeField] private ScoreManager _scoreManager;
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private PickManager _pickManager;
    private Animator _animator;

    [Header("Feelings Values")]
    [Range(0, 1)]
    [SerializeField] float _feelingBar;
    float _nextFeeling;
    bool _shouldUpdate = false;
    float _changeSpeed = 5f;

    Coroutine _currentCoroutine;
    Coroutine _mouthCoroutine;



    public void Awake()
    {
        //selection of the first eyes and mouth
        changeEyes(eyesAnimations[0].name);
        changeMouth(mouthAnimations[0].name);
        changeSpriteColor(_color);
        _animator = GetComponent<Animator>();
    }

    public void Start()
    {
        if (_scoreManager)
        {
            _scoreManager.OnCargoReachedMinimumRequirement.AddListener(CargoLeavingHandler);
            _scoreManager.OnFrustrationChanged.AddListener(UpdateFeelingBar);
            _feelingBar = _scoreManager.Frustration;
        }

        if (_gameManager)
        {
            _gameManager.OnGameStartEvent.AddListener(ResetManagerAnimations);
        }

        if (_pickManager)
        {
            _pickManager.OnPlaceWare.AddListener(PlaceWareHandler);
        }
    }

    public void Update()
    {
        //eyesDecalProjector orientation must always be align with the face center
        eyesDecalProjector.transform.LookAt(face.transform.position);
        mouthDecalProjector.transform.LookAt(face.transform.position);
        ComputeCurrentEmotion();
        ComputeColors();
        ChangeEmotion(_currentEmotion);
        changeSpriteColor(_color);

        if (_shouldUpdate)
        {
            if (_nextFeeling == _feelingBar)
            {
                _shouldUpdate = false;
            }
            int sign = (_nextFeeling > _feelingBar) ? 1 : -1;
            float time = Time.deltaTime;
            _feelingBar += time * sign * _changeSpeed;
            if (Mathf.Abs(_feelingBar - _nextFeeling) < 0.1f)
            {
                _feelingBar = _nextFeeling;
            }
        }

    }

    public void changeSpriteColor(Color color)
    {
        eyeDecalMaterial.SetColor(COLOR, color);
        mouthDecalMaterial.SetColor(COLOR, color);

        eyeDecalMaterial.SetColor(EMISSION, _emissionColor);
        mouthDecalMaterial.SetColor(EMISSION, _emissionColor);
    }


    public void changeEyes(string eyeName)
    {
        foreach (FaceAnimation eyes in eyesAnimations)
        {
            if (eyes.name == eyeName)
            {
                eyeDecalMaterial.SetTexture(BASE_MAP, eyes.texture.texture);
                eyesDecalProjector.material = eyeDecalMaterial;
            }
        }
    }

    public void changeMouth(string mouthName)
    {
        foreach (FaceAnimation mouth in mouthAnimations)
        {
            if (mouth.name == mouthName)
            {
                mouthDecalMaterial.SetTexture(BASE_MAP, mouth.texture.texture);
                mouthDecalProjector.material = mouthDecalMaterial;
            }
        }
    }


    public void ChangeEmotion(ManagerEmotions emotions)
    {
        if (_mouthCoroutine != null) return;
        foreach (EmotionDictionaryEntry emotionEntry in _emotionDictionary)
        {
            if (emotionEntry._emotion == emotions)
            {
                EmotionValues values = emotionEntry._values;
                changeEyes(eyesAnimations[values._eyes].name);
                changeMouth(mouthAnimations[values._mouth].name);
                return;
            }
        }
    }

    IEnumerator NodCoroutine(float feelingValue)
    {
        Quaternion baseRotation = transform.rotation;
        float totalTime = 0.5f;
        int nodNumber = 1;
        float differenceHalf = feelingValue - 5;
        nodNumber += +Mathf.Max(Mathf.FloorToInt(differenceHalf / 3), 0);

        float nodTime = totalTime / nodNumber;
        for (int i = 0; i < nodNumber; i++)
        {
            float percent = 0;
            while (percent < 1)
            {
                percent += Time.deltaTime / nodTime;
                Quaternion movement = (Quaternion.Euler(50 * Mathf.Sin(percent * Mathf.PI), 0, 0));
                transform.rotation = baseRotation * movement;
                yield return null;
            }
            transform.rotation = baseRotation;
        }
        _currentCoroutine = null;
    }

    IEnumerator DenyCoroutine(float feelingValue)
    {
        Quaternion baseRotation = transform.rotation;
        float totalTime = 0.25f;
        int nodNumber = 1;

        float differenceHalf = 5 - feelingValue;
        nodNumber += +Mathf.Max(Mathf.FloorToInt(differenceHalf / 3), 0);

        float nodTime = totalTime / nodNumber;
        for (int i = 0; i < nodNumber; i++)
        {
            transform.rotation = baseRotation * Quaternion.Euler(0, -15, 0);
            float percent = 0;
            while (percent < 1)
            {
                percent += Time.deltaTime / nodTime;
                Quaternion movement = (Quaternion.Euler(0, 30 * Mathf.Sin(percent * Mathf.PI), 0));
                transform.rotation = baseRotation * movement;
                yield return null;
            }
            transform.rotation = baseRotation;
        }
        _currentCoroutine = null;
    }


    void ComputeCurrentEmotion()
    {
        if (_feelingBar <= 0.2)
        {
            _currentEmotion = ManagerEmotions.Happy;
        }
        else if (_feelingBar < 0.4)
        {
            _currentEmotion = ManagerEmotions.Pleased;
        }
        else if (_feelingBar < 0.6)
        {
            _currentEmotion = ManagerEmotions.Neutral;
        }
        else if (_feelingBar < 0.8)
        {
            _currentEmotion = ManagerEmotions.Unhappy;
        }
        else
        {
            _currentEmotion = ManagerEmotions.Livid;
        }
    }

    void ComputeColors()
    {
        float lerpIndex = _feelingBar;
        _emissionColor = _gradient.Evaluate(lerpIndex);
    }

    void AngryAnimation()
    {
        if (_currentCoroutine == null)
        {
            _mouthCoroutine = StartCoroutine(MouthCoroutine());
            StartCoroutine(AngerCoroutine());
        }
    }


    private IEnumerator MouthCoroutine()
    {
        int[] tab = { 3, 2 };
        int index = 0;
        changeEyes(eyesAnimations[2].name);

        float time = 0;
        float frameTime = 0.125f;

        while (time < 2)
        {
            time += frameTime;
            int i = tab[index];
            index++;
            index %= tab.Length;
            changeMouth(mouthAnimations[i].name);
            float sign = (time < 1) ? 1 : -1;

            yield return new WaitForSeconds(frameTime);
        }
        _mouthCoroutine = null;
    }

    private IEnumerator AngerCoroutine()
    {
        float percent = 0;
        float animTime = 0.5f;
        Vector3 cameraPosition = Camera.main.transform.position - new Vector3(15.0f, 0.0f, 0.0f);
        Vector3 relativePos = cameraPosition - transform.position;
        Quaternion lookAt = Quaternion.LookRotation(relativePos);
        Quaternion oldRotation = transform.rotation;
        while (percent < 1)
        {
            percent += Time.deltaTime / animTime;
            Quaternion newRotation = Quaternion.Lerp(oldRotation, lookAt, percent);
            transform.rotation = newRotation;
            yield return null;
        }
        yield return new WaitForSeconds(1.5f);
        percent = 0;
        while (percent < 1)
        {
            percent += Time.deltaTime / animTime;
            Quaternion newRotation = Quaternion.Lerp(lookAt, oldRotation, percent);
            transform.rotation = newRotation;
            yield return null;
        }
        transform.rotation = oldRotation;
    }

    void UpdateFeelingBar(int feelingBar)
    {
        _shouldUpdate = true;
        int maxFrustration = _scoreManager.Settings.maxFrustrationAllowed;
        float oldNextFeeling = _nextFeeling;
        _nextFeeling = (float)feelingBar / (float)maxFrustration;
        if (_currentCoroutine == null)
        {
            if (_nextFeeling < oldNextFeeling)
            {
                _animator.Play("Nod", layer: 1, normalizedTime: 0f);
            }
            else
            {
                if (_nextFeeling >= 0.7 && _mouthCoroutine == null)
                {
                    AngryAnimation();
                }
                else
                {
                    _animator.Play("Deny", layer: 1, normalizedTime: 0f);
                }
            }
        }


    }

    void ResetManagerAnimations()
    {
        _feelingBar = 0;
        _nextFeeling = _feelingBar;
        _shouldUpdate = false;
    }

    void CargoLeavingHandler(bool isFilled)
    {
        if (_currentCoroutine == null && isFilled)
        {
            _animator.Play("Nod", layer: 1, normalizedTime: 0f);
        }
    }

    void PlaceWareHandler(WareEventData data)
    {
        if (_currentCoroutine == null && _feelingBar < 0.6f)
        {
            _animator.Play("Nod", layer: 1, normalizedTime: 0f);
        }
    }



}

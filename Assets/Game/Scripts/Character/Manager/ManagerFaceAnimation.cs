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
     private Animator _animator;

    [Header("Feelings Values")]
    [Range(0, 10)]
    [SerializeField] float _feelingBar;
    [SerializeField] float _cargoCompleteValue = 3;
    [SerializeField] float _cargoEmptyValue = 3;
    [SerializeField] float _dropValue = 2;
    [SerializeField] float _placeValue = 2;
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
        ScoreManager scoreManager = FindObjectOfType<ScoreManager>();
        if (scoreManager)
        {
            scoreManager.OnCargoReachedMinimumRequirement.AddListener(CargoLeavingHandler);
        }
        PickManager pickManager = FindObjectOfType<PickManager>();
        if (pickManager)
        {
            pickManager.OnDropWare.AddListener(WareDropHandler);
            pickManager.OnPlaceWare.AddListener(WarePlaceHandler);
        }
        GameManager gameManager = FindObjectOfType<GameManager>();
        if (gameManager)
        {
            gameManager.OnGameOverEvent.AddListener(ResetManagerAnimations);
        }
        _nextFeeling = _feelingBar;
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
            transform.rotation = baseRotation * Quaternion.Euler(0, -15, 0) ;
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
        if (_feelingBar <= 2)
        {
            _currentEmotion = ManagerEmotions.Livid;
        }
        else if (_feelingBar < 4)
        {
            _currentEmotion = ManagerEmotions.Unhappy;
        }
        else if (_feelingBar < 6)
        {
            _currentEmotion = ManagerEmotions.Neutral;
        }
        else if (_feelingBar < 8)
        {
            _currentEmotion = ManagerEmotions.Pleased;
        }
        else
        {
            _currentEmotion = ManagerEmotions.Happy;
        }
    }

    void ComputeColors()
    {
        float lerpIndex = _feelingBar * 0.1f;
        _emissionColor = _gradient.Evaluate(lerpIndex);
    }

    void AddFeeling(float feelingValue)
    {
        _nextFeeling += feelingValue;
        _nextFeeling = Mathf.Clamp(_nextFeeling, 0f, 10f);
        _shouldUpdate = true;
        if (_currentCoroutine == null)
        {
            if (feelingValue > 0)
            {
            //    _animator.SetTrigger("NodTrigger");
               _animator.Play("Nod", layer:1, normalizedTime:0f);
                // _currentCoroutine = StartCoroutine(NodCoroutine(_nextFeeling));
            }
            else
            {

              
                // _animator.SetTrigger("DenyTrigger");
                _animator.Play("Deny", layer:1, normalizedTime:0f);
                // _currentCoroutine = StartCoroutine(DenyCoroutine( _nextFeeling));

                if (_currentEmotion == ManagerEmotions.Livid && _mouthCoroutine == null)
                {
                    _mouthCoroutine = StartCoroutine(MouthCoroutine());
                }
            }
        }
    }

    private IEnumerator MouthCoroutine()
    {
        int[] tab = {3,2};
        int index = 0;
        changeEyes(eyesAnimations[2].name);
        float time = 0;
        float frameTime = 0.125f;
        while (time < 2)
        {
            time += frameTime;
            int i = tab[index];
            index++;
            index%= tab.Length;
            changeMouth(mouthAnimations[i].name);
            yield return new WaitForSeconds(frameTime);
        }
        _mouthCoroutine = null;
    }

    void ResetManagerAnimations()
    {
        _feelingBar = 5;
        _nextFeeling = 5;
        _shouldUpdate = false;
        _currentEmotion = ManagerEmotions.Neutral;
    }

    void CargoLeavingHandler(bool isFilled)
    {
        float feelingValue = (isFilled) ? _cargoCompleteValue : -_cargoEmptyValue;
        AddFeeling(feelingValue);
    }

    void WareDropHandler(WareEventData data)
    {
        AddFeeling(-_dropValue);
    }

    void WarePlaceHandler(WareEventData data)
    {
        AddFeeling(_placeValue);
    }


}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIWarePoints : MonoBehaviour
{
    [Header ("Settings")]
    [SerializeField] float _apparitionTime = 1f;
    [SerializeField] float _xOffset = 1f;
    [SerializeField] float _yOffset = 1f;
    [SerializeField] float _bounceForce = 1f;
    [SerializeField] AnimationCurve _yCurve;
    [SerializeField] AnimationCurve _xCurve;
    [SerializeField] AnimationCurve _scaleCurve;
    [SerializeField] float resetSeconds = 1f;


    [Header ("References")]
    [SerializeField] TMP_Text _pointsText;
    [SerializeField] Canvas _canvas;

    [SerializeField] Ware _ware;
    Camera _mainCamera;

    float _scaleStrength;

 

    Vector3 _originalPosition;

    Coroutine _testCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        _mainCamera = Camera.main;
        if (_mainCamera != null )
        {
            transform.rotation = _mainCamera.transform.rotation;
            transform.position += (_mainCamera.transform.position - transform.position).normalized * 5;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_testCoroutine == null)
        {
            _testCoroutine = StartCoroutine(AnimationCoroutine());

        }
    }

    IEnumerator AnimationCoroutine()
    {
        if (_pointsText)
        {
            _pointsText.CrossFadeAlpha(1, 0f, false);

            float percent = 0;
            while (percent < 1)
            {
                percent += Time.deltaTime / _apparitionTime;
                float xLerp = Mathf.Lerp(-5f, 5f, (Mathf.Cos((percent + 0.5f) * 2) + 1) * 0.5f);
                float yLerp = Mathf.Lerp(2.5f, 20f, _yCurve.Evaluate(percent));
                float scaleLerp = Mathf.Lerp(0.5f, 2.0f, _scaleCurve.Evaluate(percent));
                _pointsText.transform.localPosition = new Vector3(xLerp, yLerp, 0);
                _pointsText.transform.localScale = new Vector3(scaleLerp, scaleLerp, scaleLerp);
                yield return null;
            }
            _pointsText.CrossFadeAlpha(0, 0.25f, false);
        }
    }
    IEnumerator ResetCoroutine()
    {
        yield return new WaitForSeconds(resetSeconds);
        _testCoroutine = StartCoroutine(AnimationCoroutine());
    }

    public void SetPointsValue(float value)
    {
        if (_pointsText)
        {
            _pointsText.SetText(value.ToString());
        }
    }

    public void StartAnimation()
    {
        if (_pointsText)
        {
            _originalPosition = _pointsText.transform.localPosition;
        }
    }

    void SetWare(Ware ware)
    {
        _ware = ware;
    }
}

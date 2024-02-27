using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ControlRotateWareUI : MonoBehaviour
{
    Vector2 _refRes;
    CanvasGroup _canvasGroup;
    bool _hasRotated;
    Coroutine _alphaCoroutine;
    
    public void Start()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _refRes = GetComponentInParent<CanvasScaler>().referenceResolution;
        var pickManager = FindObjectOfType<PickManager>();
        pickManager.OnGrabWare.AddListener(OnGrabCargo);
        pickManager.OnDropWare.AddListener(OnDropCargo);
        pickManager.OnPlaceWare.AddListener(OnDropCargo);
        HideIcon(true);
        FindObjectOfType<GameManager>().OnGameOverEvent.AddListener(OnGameOver);
        FindObjectOfType<PickManager>().OnRotateWare.AddListener(OnRotateWare);
    }

    public void Update()
    { 
        float refRatioAspect  = _refRes.x/_refRes.y;
        float screenRatioAspect = (float)Screen.width/(float)Screen.height;
        float refWidth = _refRes.x * screenRatioAspect/refRatioAspect;
        Vector2 pos = new(Input.mousePosition.x / (float)Screen.width * refWidth - refWidth/2, 
        Input.mousePosition.y / (float)Screen.height * _refRes.y - _refRes.y /2);
        pos.x += refWidth * 0.06f;
        GetComponent<RectTransform>().localPosition = pos;
    }

    private void OnRotateWare(WareEventData arg0)
    {
        _hasRotated = true;
        HideIcon();
    }
    
    public void OnGameOver()
    {
        HideIcon();
        _hasRotated = false;
    }
    
    public void OnGrabCargo(WareEventData data)
    {
        if (_hasRotated)
        {
            return;
        }
        
        ShowIcon();
    }
    
    public void OnDropCargo(WareEventData data)
    {
        HideIcon();
    }

    private void ShowIcon(bool instant = false)
    {
        if (instant)
        {
            _canvasGroup.alpha = 1;
        }
        else
        {
            if (_alphaCoroutine != null)
            {
                StopCoroutine(_alphaCoroutine);
            }
            
            _alphaCoroutine = StartCoroutine(FadeCoroutine(1));
        }
    }

    private void HideIcon(bool instant = false)
    {
        if (instant)
        {
            _canvasGroup.alpha = 0;
        }
        else
        {
            if (_alphaCoroutine != null)
            {
                StopCoroutine(_alphaCoroutine);
            }
            
            _alphaCoroutine = StartCoroutine(FadeCoroutine(0));
        }
    }

    private IEnumerator FadeCoroutine(float targetAlpha)
    {
        float initialAlpha = _canvasGroup.alpha;
        float percent = 0;
        while (percent < 1)
        {
            percent += Time.deltaTime * 1 / .2f;

            _canvasGroup.alpha = Mathf.Lerp(initialAlpha, targetAlpha, percent);

            yield return null;
        }

        _canvasGroup.alpha = targetAlpha;
        _alphaCoroutine = null;
    }
}
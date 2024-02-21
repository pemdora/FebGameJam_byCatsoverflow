using System.Collections;
using UnityEngine;

public enum FadeType { In, Out }

public class JuiceUI : MonoBehaviour
{
    public delegate void OnFadeStarted();
    public static event OnFadeStarted OnFadeStartedAction;
    public delegate void OnFadeFinished();
    public static event OnFadeFinished OnFadeFinishedAction;
    private CanvasGroup _canvasGroup;
    [SerializeField] private float _startAlpha = 0f;
    private Coroutine _fadeCoroutine;
    [SerializeField] private AnimationCurve _fadeInCurve;
    [SerializeField] private AnimationCurve _fadeOutCurve;

    void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _canvasGroup.alpha = _startAlpha;
        _fadeCoroutine = null;
    }

    public void DoFade(FadeType fadeType)
    {
        AnimationCurve fadingCurve = fadeType switch
        {
            FadeType.In => _fadeInCurve,
            FadeType.Out => _fadeOutCurve,
            _ => _fadeInCurve
        };

        if (_canvasGroup.alpha == fadingCurve.keys[fadingCurve.length - 1].value)
        {
            return;
        }

        if (_fadeCoroutine != null)
        {
            StopCoroutine(_fadeCoroutine);
        }

        _fadeCoroutine = StartCoroutine(FadeCoroutine(fadingCurve));
        _canvasGroup.interactable = fadeType == FadeType.In;
    }

    IEnumerator FadeCoroutine(AnimationCurve animationCurve)
    {
        OnFadeStartedAction();

        float duration = animationCurve.keys[animationCurve.length - 1].time;
        float timer = duration;

        while (timer > 0)
        {
            timer -= Time.deltaTime;
            _canvasGroup.alpha = animationCurve.Evaluate((duration - timer) / duration);
            yield return null;
        }

        OnFadeFinishedAction();
        _fadeCoroutine = null;
    }
}
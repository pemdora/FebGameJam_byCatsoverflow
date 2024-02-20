using System.Collections;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject From { get; set; }
    private bool _isFading;
    private Coroutine _canvasTransitionCoroutine;

    void OnEnable()
    {
        _isFading = false;
        _canvasTransitionCoroutine = null;
        JuiceUI.OnFadeStartedAction += RegisterFadeStart;
        JuiceUI.OnFadeFinishedAction += RegisterFadeStop;
    }

    void OnDisable()
    {
        JuiceUI.OnFadeStartedAction -= RegisterFadeStart;
        JuiceUI.OnFadeFinishedAction -= RegisterFadeStop;
    }

    private void RegisterFadeStart() => _isFading = true;
    private void RegisterFadeStop() => _isFading = false;

    private IEnumerator CanvasTransitionCoroutine(GameObject source, GameObject destination)
    {
        destination.SetActive(true);
        source.GetComponent<JuiceUI>().DoFade(FadeType.Out);
        destination.GetComponent<JuiceUI>().DoFade(FadeType.In);
        while (_isFading)
        {
            yield return null;
        }
        source.SetActive(false);
        _canvasTransitionCoroutine = null;
    }

    public void ShowCanvas(GameObject destination)
    {
        if (_canvasTransitionCoroutine != null)
        {
            StopCoroutine(_canvasTransitionCoroutine);
        }
        _canvasTransitionCoroutine = StartCoroutine(CanvasTransitionCoroutine(From, destination));
    }
}
using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class ButtonScaleEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private float hoverScaleMultiplier = 1.1f;
    [SerializeField] private float hoverDuration = 0.1f;
    private Vector3 originalScale;

    private Coroutine _scaleCoroutine;


    void Start()
    {
        originalScale = transform.localScale;

        _scaleCoroutine = null;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_scaleCoroutine != null)
        {
            StopCoroutine(_scaleCoroutine);
        }

        _scaleCoroutine = StartCoroutine(ScaleOverTime(originalScale * hoverScaleMultiplier, hoverDuration));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_scaleCoroutine != null)
        {
            StopCoroutine(_scaleCoroutine);
        }
        _scaleCoroutine = StartCoroutine(ScaleOverTime(originalScale, hoverDuration));
    }

    IEnumerator ScaleOverTime(Vector3 targetScale, float duration)
    {
        Vector3 originalScale = transform.localScale;
        float startTime = Time.time;
        while (Time.time < startTime + duration)
        {
            transform.localScale = Vector3.Lerp(originalScale, targetScale, (Time.time - startTime) / duration);
            yield return null;
        }
        transform.localScale = targetScale;

        _scaleCoroutine = null;
    }
}

using System;
using System.Collections;
using UnityEngine;

namespace Game.Scripts.HUD {
    public class GameOverScreen : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float _showDuration = 0.66f;
        [SerializeField] private float _hideDuration = 0.2f;

        [Header("References")]
        [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private TMPro.TMP_Text _deliveryCount;
    [SerializeField] private TMPro.TMP_Text _scoreText;

        private Coroutine _fadeCoroutine;

    public void Show(int score, int deliveryCount)
    {
        if (_fadeCoroutine != null)
        {
            StopCoroutine(_fadeCoroutine);
        }
        _scoreText.text = score.ToString();
        _deliveryCount.text = deliveryCount.ToString();

            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
            _fadeCoroutine = StartCoroutine(FadeCoroutine(1, _showDuration));
        }

        public void Hide()
        {
            if (_fadeCoroutine != null)
            {
                StopCoroutine(_fadeCoroutine);
            }

            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
            _fadeCoroutine = StartCoroutine(FadeCoroutine(0, _hideDuration));
        }

        private IEnumerator FadeCoroutine(float targetAlpha, float duration, Action onEnd = null)
        {
            float percent = 0;
            float initialAlpha = _canvasGroup.alpha;

            while (percent < 1)
            {
                percent += Time.deltaTime * 1 / duration;

                _canvasGroup.alpha = Mathf.Lerp(initialAlpha, targetAlpha, percent);

                yield return null;
            }

            onEnd?.Invoke();
            _fadeCoroutine = null;
        }
    }
}
using System.Collections;
using TMPro;
using UnityEngine;

public class LeaderboardUserEntry : MonoBehaviour
{
    [SerializeField] private TMP_Text _userNameText;
    [SerializeField] private TMP_Text _userScoreText;
    [SerializeField] private AnimationCurve _animationCurve;
    private float _scaleDuration;
    private Coroutine _scaleCoroutine;

    private void Awake()
    {
        UnsetUserEntry();
        _scaleCoroutine = null;
    }

    private void OnEnable()
    {
        _scaleDuration = _animationCurve.keys[_animationCurve.length - 1].time;

        if (_scaleCoroutine != null)
        {
            StopCoroutine(_scaleCoroutine);
            _scaleCoroutine = null;
        }

        if (_animationCurve == null || _animationCurve.keys.Length == 0)
        {
            Debug.LogWarning($"No scale animation curve set for {name}");
        }

        _scaleCoroutine = StartCoroutine(ScaleCoroutine());
    }

    public void SetUserEntry(string userName, int userScore)
    {
        _userNameText.SetText(userName);
        _userScoreText.SetText($"{userScore} $");
    }

    public void UnsetUserEntry()
    {
        _userNameText.SetText(string.Empty);
        _userScoreText.SetText(string.Empty);
    }

    private IEnumerator ScaleCoroutine()
    {
        float timer = _scaleDuration;

        while (timer > 0)
        {
            timer -= Time.deltaTime;
            float scale = _animationCurve.Evaluate((_scaleDuration - timer) / _scaleDuration);
            transform.localScale = new Vector3(1f, scale, 1f);

            yield return null;
        }

        transform.localScale = new Vector3(1f, _animationCurve.Evaluate(_scaleDuration), 1f);

        _scaleCoroutine = null;
    }
}

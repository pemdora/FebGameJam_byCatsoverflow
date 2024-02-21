using System;
using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;

[Serializable]
public class UserSubmittedScore
{
    public string name;
    public int score;
    public int position;
}


public class GameOverScreen : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float _showDuration = 0.66f;
    [SerializeField] private float _hideDuration = 0.2f;

    [Header("References")]
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private GameObject _inputPanel;
    [SerializeField] private TMP_Text _deliveryCount;
    [SerializeField] private TMP_Text _scoreText;
    [SerializeField] private TMP_Text _failedLeaderboardText;
    [SerializeField] private TMP_InputField _inputField;
    [SerializeField] private TMP_Text _inputResultsText;

    private int _userScore;

    private Coroutine _fadeCoroutine;
    private Coroutine _apiCallCoroutine;

    public void Show(int score, int deliveryCount)
    {
        _inputPanel.SetActive(true);
        _failedLeaderboardText.SetText(string.Empty);
        _userScore = score;

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

    public void SubmitLeaderboardEntry()
    {
        if (string.IsNullOrWhiteSpace(_inputResultsText.text))
        {
            Debug.LogWarning("Cannot publish score to leaderboard because the username is not set.");
            return;
        }

        _apiCallCoroutine ??= StartCoroutine(SubmitScore());
    }

    private IEnumerator SubmitScore()
    {
        string username = _inputResultsText.text;

        WWWForm form = new();
        using UnityWebRequest www = UnityWebRequest.Post($"{LeaderboardApi.Uri}/score?user={username}&score={_userScore}", form);
        www.SetRequestHeader("Content-Type", "application/json");
        www.SetRequestHeader("Authorization", LeaderboardApi.Key);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(www.error);
        }
        else
        {
            string jsonResult = www.downloadHandler.text;
            UserSubmittedScore userSubmittedScore = JsonUtility.FromJson<UserSubmittedScore>(jsonResult);
            // TODO: Polish = envoi sur leaderboard si TOP 10, display message déso t'es nul si pas TOP 10.
        }

        _apiCallCoroutine = null;
    }
}
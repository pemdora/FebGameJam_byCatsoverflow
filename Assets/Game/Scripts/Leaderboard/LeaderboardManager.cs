using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[Serializable]
public class UserScore
{
    public string user;
    public int score;
}

[Serializable]
public class ScoreList
{
    public UserScore[] items;
}

public class LeaderboardManager : MonoBehaviour
{
    [SerializeField] private List<LeaderboardUserEntry> _leaderboardUserEntries;
    private ScoreList scoreList;
    private Coroutine _getScoreListCoroutine;
    private Coroutine _parseLeaderboardCoroutine;
    private WaitForSecondsRealtime _waitBetweenLeaderboardEntry = new(.1f);

    private void Awake()
    {
        CleanLeaderboard();
    }

    private void OnEnable()
    {
        _getScoreListCoroutine = null;
        _parseLeaderboardCoroutine = null;

        GetScoreList();
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        CleanLeaderboard();
    }

    public void OnExitButtonClicked()
    {
        gameObject.SetActive(false);
    }

    public void GetScoreList()
    {
        if (_getScoreListCoroutine != null)
        {
            StopCoroutine(_getScoreListCoroutine);
            _getScoreListCoroutine = null;
            return;
        }

        CleanLeaderboard();

        _getScoreListCoroutine = StartCoroutine(GetScoreListCoroutine());

        IEnumerator GetScoreListCoroutine()
        {
            using UnityWebRequest www = UnityWebRequest.Get($"{LeaderboardApi.Uri}/score");
            www.SetRequestHeader("Content-Type", "application/json");
            www.SetRequestHeader("Access-Control-Allow-Credentials", "true");
            www.SetRequestHeader("Access-Control-Allow-Headers", "Accept, X-Access-Token, X-Application-Name, X-Request-Sent-Time");
            www.SetRequestHeader("Access-Control-Allow-Methods", "GET, POST, OPTIONS");
            www.SetRequestHeader("Access-Control-Allow-Origin", "*");
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                CleanLeaderboard();
                Debug.LogError(www.error);
            }
            else
            {
                string jsonResult = www.downloadHandler.text;

                scoreList = JsonUtility.FromJson<ScoreList>("{\"items\":" + jsonResult + "}");

                if (_parseLeaderboardCoroutine != null)
                {
                    StopCoroutine(_parseLeaderboardCoroutine);
                    _parseLeaderboardCoroutine = null;
                }

                _parseLeaderboardCoroutine = StartCoroutine(ParseScoreList());
            }

            _getScoreListCoroutine = null;
        }
    }

    private IEnumerator ParseScoreList()
    {
        int userScoreCount = scoreList.items.Length;

        for (int i = 0; i < 10; i++)
        {
            if (i < userScoreCount)
            {
                yield return _waitBetweenLeaderboardEntry;

                UserScore userScore = scoreList.items[i];

                _leaderboardUserEntries[i].gameObject.SetActive(true);
                _leaderboardUserEntries[i].SetUserEntry(userScore.user, userScore.score);
            }
            else
            {
                yield return null;

                _leaderboardUserEntries[i].gameObject.SetActive(false);
                _leaderboardUserEntries[i].UnsetUserEntry();
            }
        }

        _parseLeaderboardCoroutine = null;
    }

    private void CleanLeaderboard()
    {
        foreach (LeaderboardUserEntry userEntry in _leaderboardUserEntries)
        {
            userEntry.UnsetUserEntry();
            userEntry.gameObject.SetActive(false);
        }
    }
}
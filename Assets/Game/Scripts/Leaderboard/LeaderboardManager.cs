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
            // To prevent making multiple API calls if one is already going.
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
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                // If GET call failed, handle it.
                CleanLeaderboard();
                Debug.LogError(www.error);
            }
            else
            {
                string jsonResult = www.downloadHandler.text;

                // Override the results for debug purpose
                // jsonResult = "[{\"user\":\"Pemdora\", \"score\":\"154\"}, {\"user\":\"pLeet\", \"score\":\"102\"}, {\"user\":\"ShidyGames\", \"score\":\"69\"}, {\"user\":\"laBlondasse\", \"score\":\"63\"}, {\"user\":\"Zakku\", \"score\":\"54\"}, {\"user\":\"Foxy WhiteTrack\", \"score\":\"49\"}, {\"user\":\"Faith\", \"score\":\"46\"}, {\"user\":\"Mizaka\", \"score\":\"41\"}, {\"user\":\"Wenzelie\", \"score\":\"36\"}, {\"user\":\"Mordilla Software\", \"score\":\"24\"}]";
                
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
            Debug.Log($"Iteration {i}, user count {userScoreCount}");

            if (i < userScoreCount)
            {
                yield return _waitBetweenLeaderboardEntry;

                UserScore userScore = scoreList.items[i];

                _leaderboardUserEntries[i].SetUserEntry(userScore.user, userScore.score);
                _leaderboardUserEntries[i].gameObject.SetActive(true);

            }
            else
            {
                yield return null;

                _leaderboardUserEntries[i].UnsetUserEntry();
                _leaderboardUserEntries[i].gameObject.SetActive(false);
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

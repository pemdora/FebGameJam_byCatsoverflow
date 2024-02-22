using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using static ScoreSettings;

public class ScoreDisplayUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Canvas _mainCanvas;
    [SerializeField] TMP_Text _scoreTxt;
    [SerializeField] TMP_Text _extraThresholdScoreTxt;
    [SerializeField] TMP_Text _extraTimerScoreTxt;

    [Header("LowScore")]
    [SerializeField] private AnimationCurve _lowScoreCurveY;
    [Header("MediumScore")]
    [SerializeField] private AnimationCurve _mediumScoreCurveY;
    [SerializeField] private AnimationCurve _mediumScaleScoreCurve;
    [Header("HighScore")]
    [SerializeField] private AnimationCurve _highScoreCurveY;
    [SerializeField] private AnimationCurve _highScaleScoreCurve;
    [Header("ExtraHighScore")]
    [SerializeField] private AnimationCurve _extraScoreCurveY;
    [SerializeField] private AnimationCurve _extraScaleScoreCurve;
    [Header("Special Bonus")]
    [SerializeField] private AnimationCurve _specialThresholdScoreCurveY; // lazy ass reuse the same for bonus timer
    [SerializeField] private AnimationCurve _specialThresholdScoreScaleCurve;

    public void DisplayWareScore(int score, Vector3 uiPosition, ScoreSettings scoreSettings)
    {
        _scoreTxt.text = "+" + score.ToString();
        Vector3 screenPos = Camera.main.WorldToScreenPoint(uiPosition);
        TMP_Text scoreTxt = Instantiate(_scoreTxt, _mainCanvas.transform);
        scoreTxt.transform.localScale = Vector3.one;

        scoreTxt.rectTransform.position = new Vector3(screenPos.x, screenPos.y);
        ScoreTresholdType scoreTresholdType = scoreSettings.GetScoreTreshold(score);

        switch (scoreTresholdType)
        {
            case ScoreTresholdType.Low:
                scoreTxt.color = scoreSettings.colorScorelow;
                break;
            case ScoreTresholdType.Medium:
                scoreTxt.color = scoreSettings.colorScoremedium;
                break;
            case ScoreTresholdType.High:
                scoreTxt.color = scoreSettings.colorScoreHigh;
                break;
            case ScoreTresholdType.ExtraHigh:
                scoreTxt.color = scoreSettings.colorScoreExtrahigh;
                break;
        }
        StartCoroutine(AnimateScore(scoreTxt, scoreTresholdType));
    }

    public void DisplayExtraThresholdBonus(int score)
    {
        _extraThresholdScoreTxt.gameObject.SetActive(true);
        _extraThresholdScoreTxt.text = "Extra Bonus!\n+" + score.ToString();
        StartCoroutine(AnimateScore(_extraThresholdScoreTxt, ScoreTresholdType.ExtraThresholdBonus));
    }

    public void DisplayTimerBonus(int score)
    {
        _extraTimerScoreTxt.gameObject.SetActive(true);
        _extraTimerScoreTxt.text = "Time Bonus!\n+" + score.ToString();
        StartCoroutine(AnimateScore(_extraTimerScoreTxt, ScoreTresholdType.ExtraThresholdBonus));
    }

    // Coroutine that move the coin along a Animation curve to the target pos
    private IEnumerator AnimateScore(TMP_Text score, ScoreTresholdType scoreTreshold)
    {
        float time = 0;
        float duration = 0;
        switch (scoreTreshold)
        {
            case ScoreTresholdType.Low:
                duration = _extraScoreCurveY.keys[_extraScoreCurveY.length - 1].time;
                break;
            case ScoreTresholdType.Medium:
                duration = _mediumScoreCurveY.keys[_mediumScoreCurveY.length - 1].time;
                break;
            case ScoreTresholdType.High:
                duration = _highScoreCurveY.keys[_highScoreCurveY.length - 1].time;
                break;
            case ScoreTresholdType.ExtraHigh:
                duration = _extraScoreCurveY.keys[_extraScoreCurveY.length - 1].time;
                break;
            case ScoreTresholdType.ExtraThresholdBonus:
                duration = _specialThresholdScoreCurveY.keys[_specialThresholdScoreCurveY.length - 1].time;
                break;
        }

        Vector3 startPos = score.rectTransform.position;
        float ratio = 0;
        while (time < duration)
        {
            ratio = time / duration;

            switch (scoreTreshold)
            {
                case ScoreTresholdType.Low:
                    score.rectTransform.position = startPos + new Vector3(0, _lowScoreCurveY.Evaluate(ratio), 0);
                    break;
                case ScoreTresholdType.Medium:
                    score.rectTransform.position = startPos + new Vector3(0, _mediumScoreCurveY.Evaluate(ratio), 0);
                    score.rectTransform.localScale = Vector3.one * _mediumScaleScoreCurve.Evaluate(ratio);
                    break;
                case ScoreTresholdType.High:
                    score.rectTransform.position = startPos + new Vector3(0, _highScoreCurveY.Evaluate(ratio), 0);
                    score.rectTransform.localScale = Vector3.one * _highScaleScoreCurve.Evaluate(ratio);
                    break;
                case ScoreTresholdType.ExtraHigh:
                    score.rectTransform.position = startPos + new Vector3(0, _extraScoreCurveY.Evaluate(ratio), 0);
                    score.rectTransform.localScale = Vector3.one * _extraScaleScoreCurve.Evaluate(ratio);
                    break;
                case ScoreTresholdType.ExtraThresholdBonus:
                    score.rectTransform.position = startPos + new Vector3(0, _specialThresholdScoreCurveY.Evaluate(ratio), 0);
                    score.rectTransform.localScale = Vector3.one * _specialThresholdScoreScaleCurve.Evaluate(ratio);
                    break;
            }
            time += Time.deltaTime;
            yield return null;
        }

        if(scoreTreshold != ScoreTresholdType.ExtraThresholdBonus)
            Destroy(score.gameObject);
        else
            score.gameObject.SetActive(false);
    }


    // private IEnumerator MoveUIToTarget(Image coinIcon)
    // {
    //     yield return null;
    //     // while (Vector3.Distance(coinIcon.rectTransform.position, target.position) > 1.5f)
    //     // {
    //     //     //coinIcon.rectTransform.position = Vector3.Lerp(coinIcon.rectTransform.position, moneyUI.rectTransform.position, 2 * Time.deltaTime);
    //     //     coinIcon.rectTransform.position = Vector3.MoveTowards(coinIcon.rectTransform.position, target.rectTransform.position, 10f);
    //     //     yield return null;
    //     // }
    //     // Destroy(coinIcon);
    // }
}

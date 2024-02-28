using System;
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
    [SerializeField] TMP_Text _perfectScoreTxt;
    [SerializeField] TMP_Text _planeScoreTxt;
    [SerializeField] Image _angryIcon;
    [SerializeField] Image _angryThresholdIcon;

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
    [SerializeField] private AnimationCurve _perfectScoreScaleCurve;
    [SerializeField] private AnimationCurve _planeScoreScaleCurve;
    [Header("Malus")]
    [SerializeField] private AnimationCurve _angryIconCurveY;
    [SerializeField] private AnimationCurve _angryIconScaleCurve;
    [SerializeField] private Transform _thresholdMalusStartPos;
    [SerializeField] private Transform _thresholdMalusEndPos;
    [SerializeField] private AnimationCurve _thresholdDurationTrajectory;
    [SerializeField] private AnimationCurve _thresholdMalusCurveY;
    [SerializeField] private AnimationCurve _thresholdMalusScaleCurve;

    public void DisplayWareScore(int score, Vector3 uiPosition, ScoreSettings scoreSettings)
    {
        _scoreTxt.text = "+" + score.ToString();
        Vector3 screenPos = Camera.main.WorldToScreenPoint(uiPosition);
        TMP_Text scoreTxt = Instantiate(_scoreTxt, _mainCanvas.transform);
        scoreTxt.transform.localScale = Vector3.one;

        scoreTxt.rectTransform.position = new Vector3(screenPos.x, screenPos.y);
        ScoreTresholdType scoreTresholdType = scoreSettings.GetScoreThreshold(score);

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

    public void DisplayAngryIcon(Vector3 uiPosition)
    {
        Image angryIcon = Instantiate(_angryIcon, _mainCanvas.transform);
        Vector3 screenPos = Camera.main.WorldToScreenPoint(uiPosition);
        angryIcon.rectTransform.position = new Vector3(screenPos.x, screenPos.y);
        StartCoroutine(AnimateAngryIcon(angryIcon));
    }

    private IEnumerator AnimateAngryIcon(Image angryIcon)
    {
        float time = 0;
        float duration = _angryIconCurveY.keys[_angryIconCurveY.length - 1].time;
        Vector3 startPos = angryIcon.rectTransform.position;
        while (time < duration)
        {
            float ratio = time / duration;
            angryIcon.rectTransform.position = startPos + new Vector3(0, _angryIconCurveY.Evaluate(ratio), 0);
            angryIcon.rectTransform.localScale = Vector3.one * _angryIconScaleCurve.Evaluate(ratio);
            time += Time.deltaTime;
            yield return null;
        }
        
        Destroy(angryIcon.gameObject);
    }


    public void DisplayExtraThresholdBonus(int score)
    {
        _extraThresholdScoreTxt.gameObject.SetActive(true);
        _extraThresholdScoreTxt.text = "Extra Bonus!\n+" + score.ToString();
        StartCoroutine(AnimateScore(_extraThresholdScoreTxt, ScoreTresholdType.ExtraThresholdBonus));
    }

    public void DisplayPerfectBonus(int score)
    {
        _perfectScoreTxt.gameObject.SetActive(true);
        _perfectScoreTxt.text = "Perfect!!!\n+" + score.ToString();
        _perfectScoreTxt.color = Color.white;
        AudioManager.Instance.PlaySoundEffect(SoundEffectType.PERFECTSCORE);
        StartCoroutine(AnimateScore(_perfectScoreTxt, ScoreTresholdType.Perfect));
    }

    public void DisplayPlaneScore(int score)
    {
        _planeScoreTxt.gameObject.SetActive(true);
        _planeScoreTxt.text = "Plane filled!\n+" + score.ToString();
        AudioManager.Instance.PlaySoundEffect(SoundEffectType.PLANESCORE);
        StartCoroutine(AnimateScore(_planeScoreTxt, ScoreTresholdType.Perfect));
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
            case ScoreTresholdType.PlaneBonus:
                duration = _planeScoreScaleCurve.keys[_planeScoreScaleCurve.length - 1].time;
                break;
            case ScoreTresholdType.Perfect:
                duration = _perfectScoreScaleCurve.keys[_perfectScoreScaleCurve.length - 1].time;
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
                case ScoreTresholdType.PlaneBonus:
                    score.rectTransform.localScale = Vector3.one * _planeScoreScaleCurve.Evaluate(ratio);
                    score.rectTransform.position = startPos + new Vector3(0, _specialThresholdScoreCurveY.Evaluate(ratio), 0);
                    score.color = Color.HSVToRGB(Mathf.PingPong(Time.time * 0.5f, 1), 1, 1);
                    break;
                case ScoreTresholdType.Perfect:
                    score.rectTransform.localScale = Vector3.one * _perfectScoreScaleCurve.Evaluate(ratio);
                    score.rectTransform.position = startPos + new Vector3(0, _specialThresholdScoreCurveY.Evaluate(ratio), 0);
                    score.color = Color.HSVToRGB(Mathf.PingPong(Time.time * 0.5f, 1), 1, 1);
                    break;
            }
            time += Time.deltaTime;
            yield return null;
        }

        if(scoreTreshold != ScoreTresholdType.ExtraThresholdBonus && scoreTreshold != ScoreTresholdType.Perfect)
            Destroy(score.gameObject);
        else
        {
            score.gameObject.SetActive(false);
        }
    }

    public void DisplayFrustrationMalus(float malusDuration, float step = 0.2f)
    {
        Vector2 screenStartPos = _thresholdMalusStartPos.position;
        Vector2 screenEndPos = _thresholdMalusEndPos.position;
        StartCoroutine(AnimateThresholdMalus(screenStartPos, screenEndPos , malusDuration, step));
    }

    private IEnumerator AnimateThresholdMalus(Vector2 screenStartPos, Vector2 screenEndPos, float malusDuration, float step)
    {
        // Instanciate the malus icon during the malus duration with a step second
        float time = 0;
        while (time < malusDuration)
        {
            Image malusIcon = Instantiate(_angryThresholdIcon, _mainCanvas.transform);
            StartCoroutine(MoveUIToTarget(malusIcon, screenStartPos, screenEndPos));
            time += step;
            yield return new WaitForSeconds(step);
        }
    }

    private IEnumerator MoveUIToTarget(Image iconToMove, Vector2 screenStartPos, Vector2 screenEndPos)
    {
        float time = 0;
        float duration = _thresholdDurationTrajectory.keys[_thresholdDurationTrajectory.length - 1].time;
        while (time < duration)
        {
            Vector3 newPos = Vector3.Lerp(screenStartPos, screenEndPos, _thresholdDurationTrajectory.Evaluate(time));
            newPos.y += _thresholdMalusCurveY.Evaluate(time);
            iconToMove.rectTransform.position = newPos;
            iconToMove.rectTransform.localScale = Vector3.one * _thresholdMalusScaleCurve.Evaluate(time);
            // iconToMove.rectTransform.position = Vector3.MoveTowards(iconToMove.rectTransform.position, iconToMove.rectTransform.position, 10f);
            time += Time.deltaTime;
            yield return null;
        }

        Destroy(iconToMove);
    }
}

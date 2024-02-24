using System;
using UnityEngine;
using Random = UnityEngine.Random;

public enum LedDisplayOrder
{
    Sequenced,
    Randomized
}

public enum LedDisplayMessageType
{
    Common,
    Success,
    Failure,
    Discard,
}

[CreateAssetMenu(menuName = "LED/LED Display Setting", fileName = "New LED Display Setting")]
public class LedDisplaySettings : ScriptableObject
{
    public LedDisplayOrder order;
    public float speed = 750;
    public float glitchDuration = 0.4f;

    [Header("Common")] 
    public Color commonTextColor;
    public string[] commonSentences;
    
    [Header("Success")] 
    public Color successTextColor;
    public string[] successSentences;
    
    [Header("Failure")] 
    public Color failureTextColor;
    public string[] failureSentences;
    
    [Header("Discard")] 
    public Color discardTextColor;
    public string[] discardSentences;

    public string GetSentence(LedDisplayMessageType type, int index)
    {
        switch (type)
        {
            case LedDisplayMessageType.Common:
                return commonSentences[index];
            case LedDisplayMessageType.Success:
                return successSentences[index];
            case LedDisplayMessageType.Failure:
                return failureSentences[index];
            case LedDisplayMessageType.Discard:
                return discardSentences[index];
        }

        return default;
    }

    public int GetRandomSentenceIndex(LedDisplayMessageType type)
    {
        switch (type)
        {
            case LedDisplayMessageType.Common:
                return Random.Range(0, commonSentences.Length);
            case LedDisplayMessageType.Success:
                return Random.Range(0, successSentences.Length);
            case LedDisplayMessageType.Failure:
                return Random.Range(0, failureSentences.Length);
            case LedDisplayMessageType.Discard:
                return Random.Range(0, discardSentences.Length);
        }

        return 0;
    }
    
    public Color GetColor(LedDisplayMessageType type)
    {
        switch (type)
        {
            case LedDisplayMessageType.Common:
                return commonTextColor;
            case LedDisplayMessageType.Success:
                return successTextColor;
            case LedDisplayMessageType.Failure:
                return failureTextColor;
            case LedDisplayMessageType.Discard:
                return discardTextColor;
        }

        return Color.magenta;
    }
}
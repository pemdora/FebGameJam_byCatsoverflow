using UnityEngine;

[CreateAssetMenu(menuName = "Score/Score Setting", fileName = "New Score Setting")]
public class ScoreSettings : ScriptableObject
{
    [Header("Frustration")]  
    [Tooltip("Amount of frustration generated per empty slot.")]
    public int frustrationPerEmptySlots = 2;
    [Tooltip("Amount of frustration for each discarded ware")]
    public int frustrationPerDiscardedWare = 10;
    [Tooltip("Max amount of frustration before getting a game over")]
    public int maxFrustrationAllowed = 100;
    [Tooltip("Relief amount of frustration when a threshold is reached")]
    public int frustrationRelief = 2;
    
    [Header("Frustration Threshold/Cargo 3")]
    [Range(0, 100)]
    [Tooltip("Threshold to reach before frustration is applied.")]
    public int frustrationThresholdCargo3Min = 25; // Eg : no more than 25% of empty slots allowed
    [Range(0, 100)]
    public int frustrationThresholdCargo3Max = 50; 
    
    [Header("Frustration Threshold/Cargo 4")]
    [Range(0, 100)]
    [Tooltip("Threshold to reach before frustration is applied.")]
    public int frustrationThresholdCargo4Min = 25; 
    [Range(0, 100)]
    public int frustrationThresholdCargo4Max = 50;
    
    [Header("Frustration Threshold/Cargo 4")]
    [Range(0, 100)]
    [Tooltip("Threshold to reach before frustration is applied.")]
    public int frustrationThresholdStep = 10; 

    [Header("Score")]
    [Tooltip("Threshold to reach before frustration is applied.")]
    public int pointsForEachSecondBeforeEndTimer = 1;
    [Tooltip("Threshold to reach before frustration is applied.")]
    public int pointsPerSlotFilled = 1;
    [Tooltip("Threshold to reach before frustration is applied.")]
    public int pointsPerExtraSlotFilled = 2;
    
    [Header("Score Thresholds Colors")]
    public int scorelow;
    public Color colorScorelow;
    public int scoremedium;
    public Color colorScoremedium;
    public int scoreHigh;
    public Color colorScoreHigh;
    public int scoreExtrahigh;
    public Color colorScoreExtrahigh;

    [Header("Adaptive difficulty")] 
    [Range(0, 100)]
    [Tooltip("How much percentage of the initial speed is added for each delivery completed")]
    public float conveyorBeltSpeedIncrease = 10;
    [Range(0, 100)]
    [Tooltip("How much percentage of the initial loading time is remove for each delivery completed")]
    public float spaceshipLoadingTimeDecrease = 10;

    public enum ScoreTresholdType
    {
        Low,
        Medium,
        High,
        ExtraHigh,
        ExtraThresholdBonus,
    }
    
    public ScoreTresholdType GetScoreThreshold(int score)
    {
        if (score <= scorelow)
        {
            return ScoreTresholdType.Low;
        }
        else if (score <= scoremedium)
        {
            return ScoreTresholdType.Medium;
        }
        else if (score <= scoreHigh)
        {
            return ScoreTresholdType.High;
        }
        else
        {
            return ScoreTresholdType.ExtraHigh;
        }
    }

    public float GetConveyorBeltSpeedIncrease()
    {
        return 1 + (conveyorBeltSpeedIncrease / 100f);
    }
}
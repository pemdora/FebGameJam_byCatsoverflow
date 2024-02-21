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
    [Header("Frustration Threshold/Cargo 3")]
    [Range(0, 100)]
    [Tooltip("Frustration penalty is applied for each empty slot past the given percentage.")]
    public int frustrationThresholdCargo3Min = 25; // Eg : no more than 25% of empty slots allowed
    [Range(0, 100)]
    public int frustrationThresholdCargo3Max = 50; 
    [Header("Frustration Threshold/Cargo 4")]
    [Range(0, 100)]
    [Tooltip("Frustration penalty is applied for each empty slot past the given percentage.")]
    public int frustrationThresholdCargo4Min = 25; 
    [Range(0, 100)]
    public int frustrationThresholdCargo4Max = 50; 
    [Header("Frustration Threshold/Cargo 4")]
    [Range(0, 100)]
    [Tooltip("Frustration Threshold step added to each cargo.")]
    public int frustrationThresholdStep = 10; 

    [Header("Score")]
    [Tooltip("Amount of points for sending out a spaceship before his timer reach zero.")]
    public int pointsForEachSecondBeforeEndTimer = 1;
    [Tooltip("How many points are rewarded for each slot filled under the threshold.")]
    public int pointsPerSlotFilled = 1;
    [Tooltip("How many points are rewarded for each slot filled above the threshold.")]
    public int pointsPerExtraSlotFilled = 2;
}
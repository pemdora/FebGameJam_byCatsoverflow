using UnityEngine;

namespace Game.Scripts.Score
{
    [CreateAssetMenu(menuName = "Score/Score Setting", fileName = "New Score Setting")]
    public class ScoreSettings : ScriptableObject
    {
        [Header("Frustration")]
        [Range(0, 100)]
        [Tooltip("Frustration penalty is applied for each empty slot past the given percentage.")]
        public int frustrationThreshold = 25;
        [Tooltip("Amount of frustration generated per empty slot.")]
        public int frustrationPerEmptySlots = 2;
        [Tooltip("Amount of frustration for each discarded ware")]
        public int frustrationPerDiscardedWare = 10;
        [Tooltip("Max amount of frustration before getting a game over")]
        public int maxFrustrationAllowed = 100;

        [Header("Score")]
        [Tooltip("Amount of points for sending out a spaceship before his timer reach zero.")]
        public int pointsForEachSecondBeforeEndTimer = 1;
        [Tooltip("How many points are rewarded for each slot filled under the threshold.")]
        public int pointsPerSlotFilled = 1;
        [Tooltip("How many points are rewarded for each slot filled above the threshold.")]
        public int pointsPerExtraSlotFilled = 2;
    }
}
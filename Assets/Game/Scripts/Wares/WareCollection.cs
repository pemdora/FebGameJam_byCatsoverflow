using UnityEngine;

namespace Game.Scripts.Wares {
    [CreateAssetMenu(menuName = "Wares/Ware Collection", fileName = "New Ware Collection")]
    public class WareCollection : ScriptableObject
    {
        public Ware[] wares;

        public Ware GetRandom()
        {
            return wares[Random.Range(0, wares.Length)];
        }
    }
}
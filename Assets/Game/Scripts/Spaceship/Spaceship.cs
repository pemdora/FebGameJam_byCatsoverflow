using UnityEngine;

namespace Game.Scripts.Spaceship {
    public class Spaceship : MonoBehaviour
    {
        [Header("Settings")]
        public int id; 
        [SerializeField] private float _loadingDuration = 10;
    
        [Header("References")] 
        [SerializeField] private Cargo.Cargo _cargo;

        public float LoadingDuration => _loadingDuration;
        public float LoadingTimer { get; private set; }
        public float LoadingLeft => LoadingDuration - LoadingTimer;
        public bool IsLoading { get; private set; }
        public Cargo.Cargo Cargo => _cargo;

        public void Initialize()
        {
            LoadingTimer = 0;
            _cargo.ResetWares();
        }

        private void Update()
        {
            if (IsLoading)
            {
                LoadingTimer += Time.deltaTime;
            }
        }

        public void StartLoading()
        {
            IsLoading = true;
        }

        public void StopLoading()
        {
            IsLoading = false;
        }
    }
}

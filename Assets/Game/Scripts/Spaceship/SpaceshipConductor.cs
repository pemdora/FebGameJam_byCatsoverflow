using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Scripts.Spaceship {
    public class SpaceshipConductor : MonoBehaviour
    {
        [Header("Settings")] 
        [SerializeField] private string[] _triggers;
    
        [Header("References")] 
        [SerializeField] private Animator _animator;
        [SerializeField] private Transform _conductor;
    
        private Spaceship _spaceship;
        private Action<Spaceship> _onArrivalCallback;

        public void AttachSpaceship(Spaceship spaceship, Action<Spaceship> onArrivalCallback, bool forcePosition)
        {
            _spaceship = spaceship;
            _spaceship.transform.SetParent(_conductor, forcePosition);
            _spaceship.transform.localPosition = Vector3.zero;
            _onArrivalCallback = onArrivalCallback;
        
            _animator.SetTrigger(_triggers[Random.Range(0, _triggers.Length)]);
        }

        public void OnAnimationEnded()
        {
            _onArrivalCallback?.Invoke(_spaceship);
            _spaceship = null;
            _onArrivalCallback = null;
        }
    }
}
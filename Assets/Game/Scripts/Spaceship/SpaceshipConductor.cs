using System;
using UnityEngine;

public class SpaceshipConductor : MonoBehaviour
{
    private static readonly int anim = Animator.StringToHash("");

    [Header("References")] 
    [SerializeField] private Animator _animator;
    
    private Spaceship _spaceship;
    private Action<Spaceship> _onArrivalCallback;

    public void AttachSpaceship(Spaceship spaceship, Action<Spaceship> onArrivalCallback)
    {
        _spaceship = spaceship;
        _spaceship.transform.SetParent(transform);
        _onArrivalCallback = onArrivalCallback;
        
        _animator.SetTrigger(anim);
    }

    public void OnAnimationEnded()
    {
        _onArrivalCallback?.Invoke(_spaceship);
        _spaceship = null;
        _onArrivalCallback = null;
    }
}
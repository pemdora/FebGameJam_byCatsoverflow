using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpaceshipConductor : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private string[] _triggers;

    [Header("References")]
    [SerializeField] private Animator _animator;
    [SerializeField] private Transform _conductor;

    private Spaceship _spaceship;
    private Action<Spaceship> _onArrivalCallback;
    private Quaternion _endRotation;

    public void AttachSpaceship(Spaceship spaceship, Action<Spaceship> onArrivalCallback, bool forcePosition)
    {
        if (!spaceship)
        {
            return;
        }

        _spaceship = spaceship;
        _endRotation = _spaceship.transform.rotation;
        _spaceship.transform.SetParent(_conductor, forcePosition);
        _spaceship.transform.localPosition = Vector3.zero;
        _onArrivalCallback = onArrivalCallback;

        _animator.SetTrigger(_triggers[Random.Range(0, _triggers.Length)]);
        _animator.Update(0);
    }

    public void OnAnimationEnded()
    {
        if (!_spaceship)
        {
            return;
        }
        
        _spaceship.transform.rotation = _endRotation;
        _onArrivalCallback?.Invoke(_spaceship);
        _spaceship = null;
        _onArrivalCallback = null;
        
        ClearChildren();
    }

    public void ClearChildren()
    {
        foreach (Transform child in _conductor)
        {
            Destroy(child.gameObject);
        }
    }
}
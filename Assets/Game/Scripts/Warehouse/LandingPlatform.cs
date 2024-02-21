using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class LandingPlatform : MonoBehaviour
{
    [Header("Inputs")]
    private PlayerControls _playerControls;

    [Header("Settings")]
    [SerializeField] private float _duration;
    [SerializeField] private AnimationCurve _ease;

    public bool CanRotate { get; set; } = true;
    public bool IsRotating => _rotationCoroutine != null;
    public float Duration => _duration;

    private Coroutine _rotationCoroutine;

    private void Awake()
    {
        _playerControls = new PlayerControls();
    }

    private void OnEnable()
    {
        _playerControls.Player.RotateLeft.Enable();
        _playerControls.Player.RotateRight.Enable();

        _playerControls.Player.RotateLeft.performed += RotateLeft;
        _playerControls.Player.RotateRight.performed += RotateRight;
    }

    private void OnDisable()
    {
        _playerControls.Player.RotateLeft.Disable();
        _playerControls.Player.RotateRight.Disable();

        _playerControls.Player.RotateLeft.performed -= RotateLeft;
        _playerControls.Player.RotateRight.performed -= RotateRight;
    }

    public void PlaceSpaceship(Spaceship spaceship)
    {
        spaceship.transform.SetParent(transform);
        spaceship.transform.position = transform.position;
    }

    private void RotateLeft(InputAction.CallbackContext context) => Rotate(true);
    private void RotateRight(InputAction.CallbackContext context) => Rotate(false);

    public void Rotate(bool clockwise)
    {
        if (!CanRotate)
        {
            return;
        }

        if (_rotationCoroutine != null)
        {
            return;
        }

        _rotationCoroutine = StartCoroutine(RotationCoroutine(clockwise ? -90 : 90));
    }

    public void ResetRotation(Action onComplete)
    {
        if (transform.rotation.y == 0)
        {
            onComplete?.Invoke();
            return;
        }

        if (_rotationCoroutine != null)
        {
            return;
        }

        float angle = transform.rotation.eulerAngles.y;
        if (transform.rotation.eulerAngles.y > 180)
        {
            angle -= 360;
        }

        _rotationCoroutine = StartCoroutine(RotationCoroutine(-Mathf.RoundToInt(angle), onComplete));
    }

    private IEnumerator RotationCoroutine(float angle, Action onComplete = null)
    {
        Quaternion initialRotation = transform.rotation;

        float percent = 0;
        while (percent < 1)
        {
            percent += Time.deltaTime * 1 / _duration;

            //transform.rotation = Quaternion.Lerp(initialRotation, initialRotation * Quaternion.Euler(0, angle * _ease.Evaluate(percent), 0), percent);
            transform.rotation = initialRotation * Quaternion.Euler(0, angle * _ease.Evaluate(percent), 0);

            yield return null;
        }

        _rotationCoroutine = null;
        onComplete?.Invoke();
    }
}
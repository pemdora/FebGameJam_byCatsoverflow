using UnityEngine;

public class Spaceship : MonoBehaviour
{
    [Header("Settings")]
    public int id;
    [SerializeField] private float _loadingDuration = 20;
    [SerializeField] private float _minLoadingDuration = 10;

    [Header("References")]
    [SerializeField] private Cargo _cargo;

    public float LoadingDuration => _loadingDuration;
    public float LoadingTimer { get; private set; }
    public float LoadingLeft => LoadingDuration - LoadingTimer;
    public bool HasLeft
    {
        get => _hasLeft;
        set => _hasLeft = value;
    }
    public bool IsLoading { get; private set; }
    public Cargo Cargo => _cargo;

    private bool _hasLeft;

    public void Initialize(float timerPenalty)
    {
        LoadingTimer = Mathf.Min(timerPenalty, _loadingDuration - _minLoadingDuration);
        _cargo.ResetWares();
        _hasLeft = false;
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

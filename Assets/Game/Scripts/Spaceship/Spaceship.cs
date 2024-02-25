using UnityEngine;

public class Spaceship : MonoBehaviour
{
    [Header("Settings")]
    public int id;
    [SerializeField] private float _loadingDuration = 20;
    [SerializeField] private float _minLoadingDuration = 10;
    [SerializeField] private SpaceshipColorScheme _colorScheme;

    [Header("References")]
    [SerializeField] private Cargo _cargo;
    [SerializeField] private Renderer[] _renderers;

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
    private static readonly int ColorShaderProperty = Shader.PropertyToID("Color");

    public void Initialize(float timerPenalty)
    {
        LoadingTimer = Mathf.Min(timerPenalty, _loadingDuration - _minLoadingDuration);
        _cargo.ResetWares();
        _hasLeft = false;
        
        RandomiseColor();
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

    private void RandomiseColor()
    {
        if (_colorScheme == null)
        {
            return;
        }
        
        Color color = _colorScheme.GetRandomColor();
        foreach (Renderer renderer in _renderers)
        {
            renderer.material.SetColor(ColorShaderProperty, color);
        }
    }
}

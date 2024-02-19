using UnityEngine;
using UnityEngine.UI;

public class FrustrationUI : MonoBehaviour
{
    [SerializeField] private Image _filler;

    private void Awake()
    {
        _filler.fillAmount = 0f;
    }

    public void UpdateFiller(float value)
    {
        _filler.fillAmount = Mathf.Clamp01(value);
    }
}
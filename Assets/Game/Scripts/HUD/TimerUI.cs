using TMPro;
using UnityEngine;
using System.Collections;

public class TimerUI : MonoBehaviour
{
    [SerializeField] private SpaceshipManager _spaceshipManager;
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private TMP_Text _timeText;
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private Color _prewarnColor;
    [SerializeField] private Color _warningColor;
    [SerializeField] private Animation _animation;
    private int _previousTime;

    private Coroutine _ticTacCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        _timeText.text = "??";
        _previousTime = 0;
        _ticTacCoroutine = null;
    }

    public void InitTimer()
    {
        InvokeRepeating(nameof(IncrementeTimer), 0, 1);
    }

    public void StopTimer()
    {
        StopCoroutine(_ticTacCoroutine);
        CancelInvoke(nameof(IncrementeTimer));
    }

    private void IncrementeTimer()
    {
        if (_spaceshipManager.HasSpaceship && _spaceshipManager.TimeRemaining > 0)
        {
            if (_previousTime + 1 == 0)
            {
                _timeText.color = Color.white;
                _rectTransform.localScale = new Vector3(1, 1, 1);

            }

            _previousTime = Mathf.FloorToInt(_spaceshipManager.TimeRemaining);
            _timeText.text = (_previousTime + 1).ToString();

            if (_previousTime + 1 == 10)
            {
                _timeText.color = _prewarnColor;
                _ticTacCoroutine = StartCoroutine(PlayTicTacSound());
            }

            if (_previousTime + 1 == _gameManager.TimeBeforeWarning)
            {
                _timeText.color = _warningColor;
                _animation.Play();
            }
        }
        else
        {
            _animation.Stop();
            _previousTime = -1;
            _timeText.text = "0";
        }
    }

    IEnumerator PlayTicTacSound()
    {
        for (int i = 0; i < 10; i++)
        {
            AudioManager.Instance.PlaySoundEffect(SoundEffectType.TIC);
            yield return new WaitForSeconds(0.5f);
            AudioManager.Instance.PlaySoundEffect(SoundEffectType.TAC);
            yield return new WaitForSeconds(0.5f);
        }
        _ticTacCoroutine = null;
    }
}

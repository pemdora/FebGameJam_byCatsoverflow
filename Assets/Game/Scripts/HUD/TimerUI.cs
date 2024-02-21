using TMPro;
using UnityEngine;

public class TimerUI : MonoBehaviour
{
    [SerializeField] private SpaceshipManager _spaceshipManager;
    [SerializeField] private TMP_Text _timeText;
    private int _previousTime;
    // Start is called before the first frame update
    void Start()
    {
        _timeText.text = "??";
        _previousTime = 0;
    }

    public void InitTimer()
    {
        InvokeRepeating(nameof(IncrementeTimer), 0, 1);
    }

    public void StopTimer()
    {
        CancelInvoke(nameof(IncrementeTimer));
    }

    private void IncrementeTimer()
    {
        if (_spaceshipManager.HasSpaceship && _spaceshipManager.TimeRemaining > 0)
        {
            _previousTime = Mathf.FloorToInt(_spaceshipManager.TimeRemaining);
            _timeText.text = (_previousTime + 1).ToString();

            if (_previousTime + 1 == 8)
            {
                AudioManager.Instance.PlaySoundEffect(SoundEffectType.TIC_TAC);
            }
        }
        else
        {
            _previousTime = 0;
            _timeText.text = "0";
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}

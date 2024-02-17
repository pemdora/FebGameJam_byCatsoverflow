using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimerUI : MonoBehaviour
{
    [SerializeField] private SpaceshipManager _spaceshipManager;
    [SerializeField] private TMP_Text _timeText;
    private float _previousTime;
    // Start is called before the first frame update
    void Start()
    {
        _timeText.text = "??";
        _previousTime = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (_spaceshipManager.IsAvailable && _spaceshipManager.TimeRemaining>0) {
            _previousTime = Mathf.FloorToInt(_spaceshipManager.TimeRemaining);
            _timeText.text = (_previousTime+1).ToString();
        }else {
            _previousTime = 0;
            _timeText.text = "0";
        }
    }
}

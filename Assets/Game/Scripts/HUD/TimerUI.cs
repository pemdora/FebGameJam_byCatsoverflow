using Game.Scripts.Spaceship;
using TMPro;
using UnityEngine;

namespace Game.Scripts.HUD {
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
            if (_spaceshipManager.HasSpaceship && _spaceshipManager.TimeRemaining>0) {
                _previousTime = Mathf.FloorToInt(_spaceshipManager.TimeRemaining);
                _timeText.text = (_previousTime+1).ToString();
                
                // TODO : add audio sound when in last seconds (TIC TAC TIC TAC TIC TAC) 
            } else {
                _previousTime = 0;
                _timeText.text = "0";
            }
        }
    }
}

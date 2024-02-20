using Game.Scripts.Spaceship;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.HUD {
    public class PercentageUI : MonoBehaviour
    {
        [SerializeField] private SpaceshipManager _spaceshipManager;
        [SerializeField] private TMP_Text _percentageText;
        [SerializeField] private Slider _objectiveSlider;
        [SerializeField] private Image _filler;
        private float _previousPercentage;

        void Start()
        {
            _objectiveSlider.value = .5f;
            _percentageText.text = "??";
            _previousPercentage = 0;
            _filler.fillAmount = 0;
        }

        void Update()
        {
            if (_spaceshipManager == null)
            {
                return;
            }

            if (_spaceshipManager.HasSpaceship && _spaceshipManager.Percentage <= 100)
            {
                _previousPercentage = Mathf.FloorToInt(_spaceshipManager.Percentage);
                _percentageText.text = _previousPercentage.ToString();
                _filler.fillAmount = _spaceshipManager.Percentage / 100f;
            }
            else
            {
                _previousPercentage = 0;
                _percentageText.text = "0";
                _filler.fillAmount = 0;
            }
        }
    }
}

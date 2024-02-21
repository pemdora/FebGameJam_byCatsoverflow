using Game.Scripts.Spaceship;
using Game.Scripts.Warehouse;
using UnityEngine;

namespace Game.Scripts.Light {
    public class LightManager : MonoBehaviour
    {

        [Header("Settings")]
        [SerializeField] private Color _warningColor;
        [SerializeField] private float _WarningLightIntensity = 5f;
    
        [Range(0, 5)]
        [SerializeField] private float _blinkLightSpeed = 1f;   
    
        [Header("Références")]
        [SerializeField] private GameManager _gameManager;
        [SerializeField] private SpaceshipManager _spaceShipManager;
        [SerializeField] private UnityEngine.Light _light;
    
        private Color _baseLightColor;
        private float _baseLightIntensity;


        // Start is called before the first frame update
        void Start()
        {
            _baseLightColor = _light.color;
            _baseLightIntensity = _light.intensity;    
        }

        public void WarningLight()
        {
            _light.color = _warningColor;
            _light.intensity = _WarningLightIntensity;
        }

        public void ResetLight()
        {
            _light.color = _baseLightColor;
            _light.intensity = _baseLightIntensity;      
        }



        // Update is called once per frame
        void Update()
        {
            if(!_spaceShipManager.HasSpaceship) return;
            if (_spaceShipManager.HasSpaceship && _spaceShipManager.TimeRemaining<_gameManager.TimeBeforeWarning) {

                
                WarningLight();

                //ping pong between 0 and 1
                float lerp = Mathf.PingPong(Time.time * _blinkLightSpeed, 1);
                _light.intensity = Mathf.Lerp(_baseLightIntensity, _WarningLightIntensity, lerp);    



            }else{
                ResetLight();
            }
        
        }
    }
}

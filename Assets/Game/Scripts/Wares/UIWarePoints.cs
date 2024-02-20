using System.Collections;
using TMPro;
using UnityEngine;
using Color = UnityEngine.Color;

namespace Game.Scripts.Wares {
    public class UIWarePoints : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] float _apparitionTime = 1f;
        [SerializeField] AnimationCurve _yCurve;
        [SerializeField] AnimationCurve _xCurve;
        [SerializeField] AnimationCurve _scaleCurve;
        [SerializeField] float resetSeconds = 1f;
        [SerializeField] float _pointValue = 50f;
        [SerializeField] float _minFinalScale = 2.0f;
        [SerializeField] float _maxFinalScale = 3.0f;
        [SerializeField] float _PointsForMaxScale = 300.0f;
        [SerializeField] bool _debugStartOnStart = false;


        [Header("References")]
        [SerializeField] TMP_Text _pointsText;
        [SerializeField] Canvas _canvas;



        Camera _mainCamera;
        float _bonusScale;




        Coroutine _testCoroutine;

        // Start is called before the first frame update
        void Start()
        {
            _mainCamera = Camera.main;
            if (_mainCamera != null)
            {
                transform.rotation = _mainCamera.transform.rotation;
                transform.position += (_mainCamera.transform.position - transform.position).normalized * 5;
            }
            if (_debugStartOnStart)
            {
                StartAnimation();
            }
        }

        IEnumerator AnimationCoroutine()
        {
            if (_pointsText)
            {
                ResetText();
                ApplyText();
                CalculateScale();

                float percent = 0;
                while (percent < 1)
                {
                    percent += Time.deltaTime / _apparitionTime;
                    float xLerp = Mathf.Lerp(-5f, 5f, (Mathf.Cos((percent + 0.5f) * 2) + 1) * 0.5f);
                    float yLerp = Mathf.Lerp(2.5f, 20f, _yCurve.Evaluate(percent));
                    float scaleLerp = Mathf.Lerp(0.5f, _minFinalScale + _bonusScale, _scaleCurve.Evaluate(percent));
                    _pointsText.transform.localPosition = new Vector3(xLerp, yLerp, 0);
                    _pointsText.transform.localScale = new Vector3(scaleLerp, scaleLerp, scaleLerp);
                    yield return null;
                }
                _pointsText.CrossFadeAlpha(0, 0.25f, false);
                Destroy(gameObject);
            }
        }
        IEnumerator ResetCoroutine()
        {
            yield return new WaitForSeconds(resetSeconds);
            _testCoroutine = StartCoroutine(AnimationCoroutine());
        }

        public void StartAnimation()
        {
            if (_testCoroutine == null)
            {
                _testCoroutine = StartCoroutine(AnimationCoroutine());
            }
        }

        private void ApplyText()
        {
            int exclamations = Mathf.Min(3, (int)(_pointValue / (_PointsForMaxScale / 3 )));
            string Text = _pointValue.ToString();
            for (int i = 0; i < exclamations; i++)
            {
                Text += "!";
            }

            Color32 color = Color.black;
            switch (exclamations)
            {
                case 1:
                    color = Color.yellow;

                    break;
                case 2:
                    color = new Color(1, 0.5f, 0);
                    break;
                case 3:
                    color = Color.red;
                    break;
            }

            _pointsText.outlineColor = color;
            _pointsText.SetText(Text);

        }

        private void ResetText()
        {
            if (_pointsText)
            {
                _pointsText.CrossFadeAlpha(1, 0f, false);
            }
        }

        private void CalculateScale()
        {
            float maxPointsDivisor = _maxFinalScale - _minFinalScale;
            int divisor = (int)(_PointsForMaxScale / maxPointsDivisor);
            _bonusScale = Mathf.Min((_pointValue - 50) / divisor,_maxFinalScale -_minFinalScale);
        }

        public void SetPointsValue(float value)
        {
            _pointValue = value;
        }

    }
}

using UnityEngine;
using UnityEngine.UI;

namespace Modelling.UI
{
    [RequireComponent(typeof(Text))]
    public sealed class FPSCounter : MonoBehaviour
    {
        [SerializeField] private Text _fpsCounter;
        [SerializeField] private float _updateInterval;

        private float _timePassedSinceLastUpdate;
        
        private void Awake()
        {
            _fpsCounter ??= GetComponent<Text>();
        }

        private void Update()
        {
            _timePassedSinceLastUpdate += Time.deltaTime;

            if (_timePassedSinceLastUpdate > _updateInterval)
            {
                float fps = 1f / Time.deltaTime;
                _fpsCounter.text = fps.ToString("F1");
                _timePassedSinceLastUpdate = 0;
            }
        }
    }
}
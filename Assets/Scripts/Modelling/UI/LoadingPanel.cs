using DG.Tweening;
using UnityEngine;

namespace Modelling.UI
{
    public class LoadingPanel : Loading
    {
        [SerializeField] private float _showHideDuration;
        [Tooltip("For proper work must be at hidden position at start")]
        [SerializeField] private Vector3 _offsetToShow;

        private bool _isShown;

        private void Start()
        {
            transform.SetParent(FindObjectOfType<Canvas>().transform, false);
        }

        public override void Show()
        {
            if (_isShown) return;

            _isShown = true;
            transform.DOMove(transform.position + _offsetToShow, _showHideDuration);
        }

        public override void Hide()
        {
            if (!_isShown) return;

            _isShown = false;
            transform.DOMove(transform.position - _offsetToShow, _showHideDuration);
        }
    }
}
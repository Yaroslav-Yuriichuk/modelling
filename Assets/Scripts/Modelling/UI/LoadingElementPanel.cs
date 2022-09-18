using System;
using DG.Tweening;
using UnityEngine;

namespace Modelling.UI
{
    public sealed class LoadingElementPanel : LoadingElement
    {
        [SerializeField] private float _showHideDuration;
        [Tooltip("For proper work must be at hidden position at start")]
        [SerializeField] private Vector3 _offsetToShow;

        private bool _isShown;

        private void Start()
        {
            transform.SetParent(FindObjectOfType<Canvas>().transform, false);
        }

        public override void Show(Action callback = null)
        {
            if (_isShown) return;

            _isShown = true;
            Sequence sequence = DOTween.Sequence();
            
            sequence
                .Append(transform.DOMove(transform.position + _offsetToShow, _showHideDuration))
                .AppendCallback(() => callback?.Invoke());

            sequence.Play();
        }

        public override void Hide(Action callback = null)
        {
            if (!_isShown) return;

            _isShown = false;
            Sequence sequence = DOTween.Sequence();

            sequence
                .Append(transform.DOMove(transform.position - _offsetToShow, _showHideDuration))
                .AppendCallback(() => callback?.Invoke());
            
            sequence.Play();
        }
    }
}
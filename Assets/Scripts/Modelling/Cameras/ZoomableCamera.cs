using System;
using Modelling.Services;
using UnityEngine;
using Zenject;

namespace Modelling.Cameras
{
    public abstract class ZoomableCamera : CameraBase
    {
        [Header("Radius constraints")]
        [SerializeField] private float _minRadius;
        [SerializeField] private float _maxRadius;
        [SerializeField] private float _radiusStep;

        protected IInputService _inputService;
        
        [Inject]
        private void Construct(IInputService inputService)
        {
            _inputService = inputService;
        }
        
        protected virtual void Update()
        {
            float mouseScroll = _inputService.GetCameraZoomScroll();
            _radius = Mathf.Clamp(_radius + _radiusStep * mouseScroll, _minRadius, _maxRadius);
        }
    }
}
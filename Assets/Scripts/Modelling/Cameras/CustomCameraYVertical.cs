using Modelling.Services;
using UnityEngine;
using Zenject;

namespace Modelling.Cameras
{
    public class CustomCameraYVertical : ZoomableCamera
    {
        [Header("Speed")]
        [SerializeField] private float _horizontalSpeed;
        [SerializeField] private float _verticalSpeed;

        protected override void Update()
        {
            base.Update();
            
            Vector2 cameraMovement = _inputService.GetCameraRotation();

            _currentRotation.y += cameraMovement.x * _horizontalSpeed;
            _currentRotation.x += cameraMovement.y * _verticalSpeed;
            
            SetPosition();
        }

        protected override void AlignAtStart()
        {
            _currentRotation = transform.rotation.eulerAngles;
            SetPosition();
        }

        private void SetPosition()
        {
            transform.localRotation = Quaternion.Euler(_currentRotation);

            float innerRadius = _radius * Mathf.Cos(ConvertToRadians(_currentRotation.x));
            
            Vector3 position = new Vector3(
                innerRadius * Mathf.Sin(ConvertToRadians(_currentRotation.y + _angleOffset.y)),
                _radius * Mathf.Sin(ConvertToRadians(_currentRotation.x)),
                innerRadius * Mathf.Cos(ConvertToRadians(_currentRotation.y + _angleOffset.y)));
            transform.localPosition = position;
        }

        private float ConvertToRadians(float angle)
        {
            return angle * Mathf.PI / 180;  
        }
    }
}
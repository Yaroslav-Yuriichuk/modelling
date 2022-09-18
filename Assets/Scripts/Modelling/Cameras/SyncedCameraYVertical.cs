using UnityEngine;

namespace Modelling.Cameras
{
    public sealed class SyncedCameraYVertical : CameraBase
    {
        [Header("Other camera")]
        [SerializeField] private CameraBase _cameraToTrack;
        [SerializeField] private bool _syncRadius;

        private void LateUpdate()
        {
            _currentRotation = _cameraToTrack.CurrentRotation;
            _radius = _cameraToTrack.Radius;
            SetPosition();
        }

        protected override void AlignAtStart()
        {
            if (_syncRadius)
            {
                _radius = _cameraToTrack.Radius;
            }
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
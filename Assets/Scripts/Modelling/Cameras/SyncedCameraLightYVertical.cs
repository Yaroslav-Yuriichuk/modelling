using UnityEngine;

namespace Modelling.Cameras
{
    public sealed class SyncedCameraLightYVertical : MonoBehaviour
    {
        [SerializeField] private CameraBase _cameraToTrack;
        
        [Header("Angles")]
        [SerializeField] private Vector3 _startRotation;
        [SerializeField] private Vector3 _angleOffset;

        private void Start()
        {
            transform.rotation = Quaternion.Euler(_startRotation);
        }

        private void LateUpdate()
        {
            Vector3 rotation = transform.rotation.eulerAngles;

            rotation.y = _cameraToTrack.CurrentRotation.y + _angleOffset.y;
            transform.rotation = Quaternion.Euler(rotation);
        }
    }
}
using UnityEngine;

namespace Modelling.Cameras
{
    public abstract class CameraBase : MonoBehaviour
    {
        public Vector3 CurrentRotation => _currentRotation;
        public float Radius => _radius;
        
        [SerializeField] protected float _radius;

        [Header("Angles")]
        [SerializeField] private Vector3 _startRotation;
        [SerializeField] protected Vector3 _angleOffset;

        protected Vector3 _currentRotation;

        private void Start()
        {
            transform.rotation = Quaternion.Euler(_startRotation);
            AlignAtStart();
        }

        protected abstract void AlignAtStart();
    }
}
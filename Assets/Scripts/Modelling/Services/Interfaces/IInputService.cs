using UnityEngine;

namespace Modelling.Services
{
    public interface IInputService
    {
        public bool GetIntrusionPoint(out Vector3 point);
        public bool GetExtrusionPoint(out Vector3 point);

        public bool WantsToUndo();

        public Vector2 GetCameraRotation();
        public float GetCameraZoomScroll();
    }
}
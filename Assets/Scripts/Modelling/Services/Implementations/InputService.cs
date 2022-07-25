using UnityEngine;
using Zenject;

namespace Modelling.Services
{
    public class InputService : IInputService, ITickable
    {
        private Camera _camera;

        private const float AltCooldownTime = 0.2f;
        private bool _wasAltPressed;
        private float _timePassedSinceAltReleased;
        
        public InputService()
        {
            _camera = Camera.main;
        }
        
        public bool GetIntrusionPoint(out Vector3 point)
        {
            point = Vector3.zero;
            if (_wasAltPressed && _timePassedSinceAltReleased < AltCooldownTime) return false;
            
            _wasAltPressed = false;
            _timePassedSinceAltReleased = 0;

            if (!Input.GetMouseButtonUp(0) || Input.GetKey(KeyCode.LeftAlt)) return false;

            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            if (!Physics.Raycast(ray, out RaycastHit hit)) return false;

            point = hit.point;
            return true;
        }

        public bool GetExtrusionPoint(out Vector3 point)
        {
            point = Vector3.zero;
            if (_wasAltPressed && _timePassedSinceAltReleased < AltCooldownTime) return false;
                
            _wasAltPressed = false;
            _timePassedSinceAltReleased = 0;

            if (!Input.GetMouseButtonUp(1) || Input.GetKey(KeyCode.LeftAlt)) return false;

            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            if (!Physics.Raycast(ray, out RaycastHit hit)) return false;

            point = hit.point;
            return true;
        }

        public bool WantsToUndo()
        {
            return Input.GetKey(KeyCode.LeftControl) && Input.GetKeyUp(KeyCode.Z);
        }

        public Vector2 GetCameraRotation()
        {
            if (!Input.GetKey(KeyCode.LeftAlt) || !Input.GetMouseButton(0)) return Vector2.zero;
            return new Vector2(Input.GetAxis("Mouse X"), -Input.GetAxis("Mouse Y"));
        }

        public float GetCameraZoomScroll()
        {
            return -Input.mouseScrollDelta.y;
        }

        public void Tick()
        {
            bool isAltPressed = Input.GetKey(KeyCode.LeftAlt);

            if (_wasAltPressed && !isAltPressed)
            {
                _timePassedSinceAltReleased += Time.deltaTime;
                return;
            }

            if (!_wasAltPressed && isAltPressed)
            {
                _wasAltPressed = true;
            }
        }
    }
}
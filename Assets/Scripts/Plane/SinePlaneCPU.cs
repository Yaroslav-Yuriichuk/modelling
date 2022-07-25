using UnityEngine;

namespace Planes
{
    public class SinePlaneCPU : SinePlane
    {
        protected override void ApplySine()
        {
            Vector3 determiningVector = new Vector3(
                Mathf.Cos(_angle * Mathf.PI / 180f), 0f, Mathf.Sin(_angle * Mathf.PI / 180f));

            for (int i = 0; i < _vertices.Length; i++)
            {
                _vertices[i].y = _amplitude * Mathf.Sin(
                    Vector3.Dot(_vertices[i] - _pivotVertex, determiningVector) + Time.time * _period);
            }

            _meshFilter.mesh.vertices = _vertices;
        }
    }
}
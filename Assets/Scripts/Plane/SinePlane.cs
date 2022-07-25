using UnityEngine;

namespace Planes
{
    [RequireComponent(typeof(MeshFilter))]
    public abstract class SinePlane : MonoBehaviour
    {
        [SerializeField] protected MeshFilter _meshFilter;

        [Header("Adjust")]
        [SerializeField] protected float _period;
        [Range(0f, 1f)]
        [SerializeField] protected float _amplitude;
        [Range(-180f, 180f)]
        [SerializeField] protected float _angle;
        
        protected Vector3[] _vertices;
        protected Vector3 _pivotVertex;

        private void Awake()
        {
            _meshFilter ??= GetComponent<MeshFilter>();
            Debug.Log($"Total amount of vertices {_meshFilter.mesh.vertices.Length}");
        }

        protected virtual void OnEnable()
        {
            _vertices = _meshFilter.mesh.vertices;
            DeterminePivotVertex();
        }

        private void DeterminePivotVertex()
        {
            Vector3 determiningVector = new Vector3(
                Mathf.Cos(_angle * Mathf.PI / 180f), 0f, Mathf.Sin(_angle * Mathf.PI / 180f));

            _pivotVertex = _vertices[0];
            foreach (var vertex in _vertices)
            {
                if (Vector3.Dot(determiningVector, vertex) < Vector3.Dot(determiningVector, _pivotVertex))
                {
                    _pivotVertex = vertex;
                }   
            }
        }

        private void Update()
        {
            ApplySine();
        }

        protected abstract void ApplySine();
    }
}

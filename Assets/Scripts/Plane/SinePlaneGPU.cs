using UnityEngine;

namespace Planes
{
    public class SinePlaneGPU : SinePlane
    {
        [SerializeField] private ComputeShader _sineShader;
        
        private ComputeBuffer _computeBuffer;
        
        private const int ThreadsNumber = 1000;
        private int _threadGroupsX;
        
        protected override void OnEnable()
        {
            base.OnEnable();
            
            _threadGroupsX = (_vertices.Length + ThreadsNumber - 1) / ThreadsNumber;
            
            _sineShader.SetInt("number_of_vertices", _vertices.Length);
            _sineShader.SetFloat("angle", _angle);
            _sineShader.SetFloat("period", _period);
            _sineShader.SetFloat("amplitude", _amplitude);
            _sineShader.SetFloats("pivot_vertex", _pivotVertex.x, _pivotVertex.y, _pivotVertex.z);
            
            _computeBuffer = new ComputeBuffer(_vertices.Length, sizeof(float) * 3);
            
            _computeBuffer.SetData(_vertices);
            _sineShader.SetBuffer(0, "vertices", _computeBuffer);
        }

        private void OnDisable()
        {
            _computeBuffer.Dispose();
        }

        protected override void ApplySine()
        {
            _sineShader.SetFloat("time", Time.time);
            _sineShader.Dispatch(0, _threadGroupsX, 1, 1);
            
            _computeBuffer.GetData(_vertices);

            _meshFilter.mesh.vertices = _vertices;
        }
    }
}
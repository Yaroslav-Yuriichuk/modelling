using Modelling.Services;
using UnityEngine;
using ILogger = Modelling.Services.ILogger;

namespace Modelling
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshCollider))]
    public class ChunkObject : MonoBehaviour
    {
        public Model Model { get; private set; }
        public Chunk Chunk { get; private set; }

        private bool _updateCollider;
        
        [SerializeField] private MeshFilter _meshFilter;
        [SerializeField] private MeshCollider _meshCollider;

        private IBuildingService _buildingService;
        private ILogger _logger;
        
        private void Awake()
        {
            _meshFilter ??= GetComponent<MeshFilter>();
            _meshCollider ??= GetComponent<MeshCollider>();
        }

        public void Init(IBuildingService buildingService, ILogger logger, bool updateCollider)
        {
            _buildingService = buildingService;
            _logger = logger;
            _updateCollider = updateCollider;
        }
        
        public void ApplyChunk(Chunk chunk, Model model)
        {
            Model = model;
            Chunk = chunk;

            ChunkBuildResult result = _buildingService.Build(chunk, model);
            if (!result.IsSuccessful) return;
            
            _meshFilter.mesh = result.ChunkMesh;
            if (_updateCollider)
            {
                _meshCollider.sharedMesh = result.ChunkMesh;
            }
        }
    }
}
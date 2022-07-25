using Modelling.Services;
using UnityEngine;
using Zenject;
using ILogger = Modelling.Services.ILogger;

namespace Modelling
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshCollider))]
    public class ModelObject : MonoBehaviour
    {
        public Model Model { get; private set; }

        public bool UpdateCollider { get; set; } = true;
        
        [SerializeField] private MeshFilter _meshFilter;
        [SerializeField] private MeshCollider _meshCollider;

        private ILogger _logger;
        private IModelBuildingService _modelBuildingService;

        [Inject]
        private void Construct(IModelBuildingService modelBuildingService, ILogger logger)
        {
            _modelBuildingService = modelBuildingService;
            _logger = logger;
        }
        
        private void Awake()
        {
            _meshFilter ??= GetComponent<MeshFilter>();
            _meshCollider ??= GetComponent<MeshCollider>();
        }

        public void ApplyModel(Model model)
        {
            _logger.Log("Applying model");
            
            Model = model;
            Mesh newMesh = _modelBuildingService.Build(model);
            
            if (newMesh == null) return;
            
            _meshFilter.mesh = newMesh;
            if (UpdateCollider)
            {
                _meshCollider.sharedMesh = newMesh;   
            }
        }
    }
}
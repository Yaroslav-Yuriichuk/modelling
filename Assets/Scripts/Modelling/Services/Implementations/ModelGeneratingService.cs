using UnityEngine;

namespace Modelling.Services
{
    public class ModelGeneratingService : IModelGeneratingService
    {
        private readonly ILogger _logger;
        
        public ModelGeneratingService(ILogger logger)
        {
            _logger = logger;
        }
        
        public Model GenerateModel(ModelConstraints constraints)
        {
            _logger.Log($"Started generating {constraints.Type}");
            
            string path = $"Compute Shaders/Generating/GeneratingShader-{constraints.Type}";
            ComputeShader shader = Resources.Load<ComputeShader>(path);

            shader.SetInts("model_size", constraints.Width, constraints.Height, constraints.Length);
            shader.SetFloat("voxel_size", constraints.VoxelSize);
            
            ComputeBuffer voxelsBuffer =
                new ComputeBuffer(constraints.Width * constraints.Height * constraints.Length, sizeof(int));

            VoxelData[] voxels = new VoxelData[constraints.Width * constraints.Height * constraints.Length];

            shader.SetBuffer(0, "voxels", voxelsBuffer);
            shader.Dispatch(0, 
                GetGroupsCount(constraints.Width),
                GetGroupsCount(constraints.Height),
                GetGroupsCount(constraints.Length));

            voxelsBuffer.GetData(voxels);
            voxelsBuffer.Dispose();
            
            return new Model(voxels, constraints.Width, constraints.Height, constraints.Length, constraints.VoxelSize);
        }

        private int GetGroupsCount(int modelSize)
        {
            const int threadsInEachDirection = 10;
            return (modelSize + threadsInEachDirection - 1) / threadsInEachDirection;
        }
    }
}
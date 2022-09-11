using UnityEngine;

namespace VoxelModelling.Services
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
            string path = $"Compute Shaders/Generating/GeneratingShader-{constraints.Type}";
            ComputeShader shader = Resources.Load<ComputeShader>(path);

            shader.SetInts("model_size", constraints.Width, constraints.Height, constraints.Length);
            shader.SetInts("chunk_size", Chunk.Width, Chunk.Height, Chunk.Length);
            shader.SetFloat("voxel_size", constraints.VoxelSize);

            ComputeBuffer voxelsBuffer = new ComputeBuffer(Chunk.TotalVolume, sizeof(int));
            VoxelData[] voxels = new VoxelData[Chunk.TotalVolume];

            Size3D sizeInVoxels = Chunk.GetSizeInChunks(constraints.Size);
            Chunk[] chunks = new Chunk[sizeInVoxels.TotalVolume];

            shader.SetBuffer(0, "voxels", voxelsBuffer);
            
            for (int z = 0; z < sizeInVoxels.Length; z++)
            {
                for (int y = 0; y < sizeInVoxels.Height; y++)
                {
                    for (int x = 0; x < sizeInVoxels.Width; x++)
                    {
                        shader.SetInts("chunk_id", x, y, z);

                        shader.Dispatch(0,
                            GetGroupsCount(Chunk.Width),
                            GetGroupsCount(Chunk.Height),
                            GetGroupsCount(Chunk.Length));
                        
                        voxelsBuffer.GetData(voxels);
                        chunks[sizeInVoxels.GetLinearIndex(x, y, z)]
                            = new Chunk(new ChunkId(x, y, z), (VoxelData[])voxels.Clone());
                    }
                }
            }

            voxelsBuffer.Dispose();
            
            return new Model(chunks, constraints.Width, constraints.Height, constraints.Length, constraints.VoxelSize);
        }

        private int GetGroupsCount(int modelSize)
        {
            const int threadsInEachDirection = 10;
            return (modelSize + threadsInEachDirection - 1) / threadsInEachDirection;
        }
    }
}
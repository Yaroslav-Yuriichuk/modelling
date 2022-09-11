using UnityEngine;

namespace VoxelModelling.Services
{
    public struct ChunkBuildResult
    {
        public bool IsSuccessful;
        public Mesh ChunkMesh;

        public ChunkBuildResult(bool isSuccessful, Mesh chunkMesh)
        {
            IsSuccessful = isSuccessful;
            ChunkMesh = chunkMesh;
        }
    }
    
    public interface IBuildingService
    {
        public ChunkBuildResult Build(Chunk chunk, Model model);
    }
}
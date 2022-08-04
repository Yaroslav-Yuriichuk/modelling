using UnityEngine;

namespace Modelling.Geometry
{
    public class Cube : ModificationShape
    {
        private Vector3 _center;
        private readonly float _side;
        
        public Cube(Vector3 center, float side)
        {
            _center = center;
            _side = side;
        }
        
        public override void MoveBy(Vector3 offset)
        {
            _center += offset;
        }

        public override bool Intersects(Chunk chunk, float voxelSize)
        {
            Vector3 chunkOffset = new Vector3(
                voxelSize * chunk.Id.X * Chunk.Width,
                voxelSize * chunk.Id.Y * Chunk.Height,
                voxelSize * chunk.Id.Z * Chunk.Length);

            return AnyChunkVertexInCube(voxelSize, chunkOffset)
                   || AnyCubeVertexInChunk(voxelSize, chunkOffset);
        }

        private bool AnyCubeVertexInChunk(float voxelSize, Vector3 chunkOffset)
        {
            bool IsPointInChunk(Vector3 point)
            {
                bool isInside;

                isInside = (chunkOffset.x - point.x) * (chunkOffset.x + voxelSize * Chunk.Width - point.x) <= 0;
                isInside = isInside && (chunkOffset.y - point.y) * (chunkOffset.y + voxelSize * Chunk.Height - point.y) <= 0;
                isInside = isInside && (chunkOffset.z - point.z) * (chunkOffset.z + voxelSize * Chunk.Length - point.z) <= 0;
            
                return isInside;
            }

            return IsPointInChunk(_center + _side / 2 * new Vector3(1, 1, 1))
                   || IsPointInChunk(_center + _side / 2 * new Vector3(1, 1, -1))
                   || IsPointInChunk(_center + _side / 2 * new Vector3(1, -1, 1))
                   || IsPointInChunk(_center + _side / 2 * new Vector3(1, -1, -1))
                   || IsPointInChunk(_center + _side / 2 * new Vector3(-1, 1, 1))
                   || IsPointInChunk(_center + _side / 2 * new Vector3(-1, 1, -1))
                   || IsPointInChunk(_center + _side / 2 * new Vector3(-1, -1, 1))
                   || IsPointInChunk(_center + _side / 2 * new Vector3(-1, -1, -1));
        }

        private bool AnyChunkVertexInCube(float voxelSize, Vector3 chunkOffset)
        {
            bool IsPointInCube(Vector3 point)
            {
                bool isInside;

                isInside = (_center.x - _side / 2 - point.x) * (_center.x + _side / 2 - point.x) <= 0;
                isInside = isInside && (_center.y - _side / 2 - point.y) * (_center.y + _side / 2 - point.y) <= 0;
                isInside = isInside && (_center.z - _side / 2 - point.z) * (_center.z + _side / 2 - point.z) <= 0;

                return isInside;
            }

            return IsPointInCube(chunkOffset)
                   || IsPointInCube(chunkOffset + voxelSize * new Vector3(Chunk.Width, 0, 0))
                   || IsPointInCube(chunkOffset + voxelSize * new Vector3(0, Chunk.Height, 0))
                   || IsPointInCube(chunkOffset + voxelSize * new Vector3(0, 0, Chunk.Length))
                   || IsPointInCube(chunkOffset + voxelSize * new Vector3(Chunk.Width, Chunk.Height, 0))
                   || IsPointInCube(chunkOffset + voxelSize * new Vector3(Chunk.Width, 0, Chunk.Length))
                   || IsPointInCube(chunkOffset + voxelSize * new Vector3(0, Chunk.Height, Chunk.Length))
                   || IsPointInCube(chunkOffset + voxelSize * new Vector3(Chunk.Width, Chunk.Height, Chunk.Length));
        }
    }
}
using System;

namespace VoxelModelling
{
    public struct ChunkId
    {
        public int X;
        public int Y;
        public int Z;

        public ChunkId(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }
    }

    [Serializable]
    public struct Size3D
    {
        public int Width;
        public int Height;
        public int Length;

        public int TotalVolume => Width * Height * Length;
        
        public Size3D(int width, int height, int length)
        {
            Width = width;
            Height = height;
            Length = length;
        }

        public int GetLinearIndex(int x, int y, int z)
        {
            return z * Width * Height + y * Width + x;
        }

        public bool IsChunkable()
        {
            return Width % Chunk.Width == 0 && Height % Chunk.Height == 0 && Length % Chunk.Length == 0;
        }
    }
    
    public class Chunk
    {
        public const int Width = 50;
        public const int Height = 50;
        public const int Length = 50;

        public const int TotalVolume = Width * Height * Length;

        public ChunkId Id { get; }
        public VoxelData[] Voxels { get; }

        public Chunk(ChunkId id, VoxelData[] voxels)
        {
            Id = id;
            Voxels = voxels;
        }

        public static Size3D GetSizeInChunks(Size3D size)
        {
            return new Size3D(
                (size.Width + Width - 1) / Width,
                (size.Height + Height - 1) / Height,
                (size.Length + Length - 1) / Length);
        }
        
        public VoxelData GetVoxelAt(int x, int y, int z)
        {
            if (x >= Width || y >= Height || z >= Length || x < 0 || y < 0 || z < 0)
            {
                return VoxelData.None;
            }
            
            return Voxels[z * Width * Height + y * Width + x];
        }

        public int GetVoxelIndex(int x, int y, int z)
        {
            if (x >= Width || y >= Height || z >= Length || x < 0 || y < 0 || z < 0)
            {
                return -1;
            }

            return z * Width * Height + y * Width + x;
        }
    }
}
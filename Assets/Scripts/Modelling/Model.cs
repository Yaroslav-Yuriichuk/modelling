using System;

namespace Modelling
{
    public struct VoxelData
    {
        public int Exists;

        public static VoxelData None = new VoxelData() { Exists = 0 };
    }
    
    public class Model
    {
        public VoxelData[] Voxels { get; }

        public int Width { get; }  // X
        public int Height { get; } // Y
        public int Length { get; } // Z

        public float VoxelSize { get; }

        public int TotalVolume => Width * Height * Length;
        
        public Model(VoxelData[] voxels, int width, int height, int length, float voxelSize)
        {
            Voxels = voxels;
            
            Width = width;
            Height = height;
            Length = length;

            VoxelSize = voxelSize;
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
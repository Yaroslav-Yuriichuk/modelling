using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VoxelModelling.Geometry;

namespace VoxelModelling
{
    public struct VoxelData
    {
        public int Exists;

        public static VoxelData None = new VoxelData { Exists = 0 };
    }
    
    public class Model
    {
        public Chunk[] Chunks { get; }
        
        public int Width { get; }  // X
        public int Height { get; } // Y
        public int Length { get; } // Z

        public float VoxelSize { get; }

        public int TotalVolume => Width * Height * Length;
        
        public Size3D Size => new Size3D(Width, Height, Length);
        public Size3D SizeInChunks => Chunk.GetSizeInChunks(Size);
        
        public Model(Chunk[] chunks, int width, int height, int length, float voxelSize)
        {
            Chunks = chunks;
            
            Width = width;
            Height = height;
            Length = length;

            VoxelSize = voxelSize;
        }

        public Chunk GetChunkAt(ChunkId id) => GetChunkAt(id.X, id.Y, id.Z);
        public Chunk GetChunkAt(int x, int y, int z)
        {
            if (x >= SizeInChunks.Width || y >= SizeInChunks.Height || z >= SizeInChunks.Length || x < 0 || y < 0 || z < 0)
            {
                return null;
            }
            
            return Chunks[z * SizeInChunks.Width * SizeInChunks.Height + y * SizeInChunks.Width + x];
        }

        public int GetChunkIndex(ChunkId id) => GetChunkIndex(id.X, id.Y, id.Z);
        public int GetChunkIndex(int x, int y, int z)
        {
            if (x >= SizeInChunks.Width || y >= SizeInChunks.Height || z >= SizeInChunks.Length || x < 0 || y < 0 || z < 0)
            {
                return -1;
            }

            return z * SizeInChunks.Width * SizeInChunks.Height + y * SizeInChunks.Width + x;
        }
        
        public IEnumerable<ChunkId> GetInvolvedChunksId(ModificationShape shape)
        {
            DateTime before = DateTime.Now;
            Vector3 offset = new Vector3(
                VoxelSize * Width / 2,
                VoxelSize * Height / 2,
                VoxelSize * Length / 2);
            shape.MoveBy(offset);
            
            return Chunks.Where(chunk => shape.Intersects(chunk, VoxelSize)).Select(chunk => chunk.Id).ToArray();
        }
    }
}
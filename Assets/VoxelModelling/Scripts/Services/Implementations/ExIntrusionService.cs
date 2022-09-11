using System;
using System.Collections.Generic;
using UnityEngine;
using VoxelModelling.Extensions;
using VoxelModelling.Geometry;

namespace VoxelModelling.Services
{
    public class ExIntrusionService : IExIntrusionService
    {
        public float IntrusionStrength { get; set; } = 0.25f;
        public float ExtrusionStrength { get; set; } = 0.25f;

        public ShapeType IntrusionShapeType { get; set; }
        public ShapeType ExtrusionShapeType { get; set; }

        private readonly Dictionary<ShapeType, ComputeShader> _intrusionShaders = new Dictionary<ShapeType, ComputeShader>();
        private readonly Dictionary<ShapeType, ComputeShader> _extrusionShaders = new Dictionary<ShapeType, ComputeShader>();

        public ExIntrusionService()
        {
            foreach (ShapeType shapeType in Enum.GetValues(typeof(ShapeType)))
            {
                string intrusionShaderPath = $"Compute Shaders/IntrusionExtrusion/IntrusionShader-{shapeType}";
                string extrusionShaderPath = $"Compute Shaders/IntrusionExtrusion/ExtrusionShader-{shapeType}";
                
                ComputeShader intrusionShader = Resources.Load<ComputeShader>(intrusionShaderPath);
                ComputeShader extrusionShader = Resources.Load<ComputeShader>(extrusionShaderPath);
                
                intrusionShader.SetInts("chunk_size", Chunk.Width, Chunk.Height, Chunk.Length);
                extrusionShader.SetInts("chunk_size", Chunk.Width, Chunk.Height, Chunk.Length);
                
                _intrusionShaders.Add(shapeType, intrusionShader);
                _extrusionShaders.Add(shapeType, extrusionShader);
            }
        }

        public IntrusionResult Intrude(Model model, Vector3 point)
        {
            ComputeShader intrusionShader = _intrusionShaders[IntrusionShapeType];
            
            intrusionShader.SetInts("model_size", model.Width, model.Height, model.Length);
            intrusionShader.SetFloat("voxel_size", model.VoxelSize);
            
            intrusionShader.SetFloat("intrusion_strength", IntrusionStrength);
            intrusionShader.SetFloats("intrusion_point", point.x, point.y, point.z);
            
            ComputeBuffer voxelsBuffer = new ComputeBuffer(Chunk.TotalVolume, sizeof(int));

            Chunk[] chunks = (Chunk[]) model.Chunks.Clone();

            ModificationShape intrusionShape = IntrusionShapeType.Construct(point, IntrusionStrength);
            var chunksIdToModify = model.GetInvolvedChunksId(intrusionShape);

            foreach (var id in chunksIdToModify)
            {
                VoxelData[] voxels = (VoxelData[]) model.GetChunkAt(id).Voxels.Clone();
                
                voxelsBuffer.SetData(voxels);
            
                intrusionShader.SetInts("chunk_id", id.X, id.Y, id.Z);
                intrusionShader.SetBuffer(0, "voxels", voxelsBuffer);
                
                intrusionShader.Dispatch(0, 
                    GetGroupsCount(Chunk.Width),
                    GetGroupsCount(Chunk.Height),
                    GetGroupsCount(Chunk.Length));
            
                voxelsBuffer.GetData(voxels);
                chunks[model.GetChunkIndex(id)] = new Chunk(id, voxels);
            }
            
            voxelsBuffer.Dispose();

            Model newModel = new Model(chunks, model.Width, model.Height, model.Length, model.VoxelSize);
            return new IntrusionResult(newModel, chunksIdToModify);
        }

        public ExtrusionResult Extrude(Model model, Vector3 point)
        {
            ComputeShader extrusionShader = _extrusionShaders[ExtrusionShapeType];
            
            extrusionShader.SetInts("model_size", model.Width, model.Height, model.Length);
            extrusionShader.SetFloat("voxel_size", model.VoxelSize);
            
            extrusionShader.SetFloat("extrusion_strength", ExtrusionStrength);
            extrusionShader.SetFloats("extrusion_point", point.x, point.y, point.z);
            
            ComputeBuffer voxelsBuffer = new ComputeBuffer(Chunk.TotalVolume, sizeof(int));

            Chunk[] chunks = (Chunk[]) model.Chunks.Clone();

            ModificationShape extrusionShape = ExtrusionShapeType.Construct(point, ExtrusionStrength);
            var chunksIdToModify = model.GetInvolvedChunksId(extrusionShape);
            
            foreach (var id in chunksIdToModify)
            {
                VoxelData[] voxels = (VoxelData[]) model.GetChunkAt(id).Voxels.Clone();
                
                voxelsBuffer.SetData(voxels);
            
                extrusionShader.SetInts("chunk_id", id.X, id.Y, id.Z);
                extrusionShader.SetBuffer(0, "voxels", voxelsBuffer);
                
                extrusionShader.Dispatch(0, 
                    GetGroupsCount(Chunk.Width),
                    GetGroupsCount(Chunk.Height),
                    GetGroupsCount(Chunk.Length));
            
                voxelsBuffer.GetData(voxels);
                chunks[model.GetChunkIndex(id)] = new Chunk(id, voxels);
            }
            
            voxelsBuffer.Dispose();

            Model newModel = new Model(chunks, model.Width, model.Height, model.Length, model.VoxelSize);
            return new ExtrusionResult(newModel, chunksIdToModify);
        }
        
        private int GetGroupsCount(int modelSize)
        {
            const int threadsInEachDirection = 10;
            return (modelSize + threadsInEachDirection - 1) / threadsInEachDirection;
        }
    }
}
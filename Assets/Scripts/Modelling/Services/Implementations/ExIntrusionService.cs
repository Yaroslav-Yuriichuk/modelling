using System;
using Modelling.Geometry;
using UnityEngine;

namespace Modelling.Services
{
    public class ExIntrusionService : IExIntrusionService
    {
        public float IntrusionStrength { get; set; } = 0.25f;
        public float ExtrusionStrength { get; set; } = 0.25f;
        
        private readonly ILogger _logger;
        private readonly ComputeShader _intrusionShader;
        private readonly ComputeShader _extrusionShader;
        
        public ExIntrusionService(ILogger logger)
        {
            _logger = logger;
            
            const string intrusionShaderPath = "Compute Shaders/IntrusionExtrusion/IntrusionShader";
            const string extrusionShaderPath = "Compute Shaders/IntrusionExtrusion/ExtrusionShader";

            _intrusionShader = Resources.Load<ComputeShader>(intrusionShaderPath);
            _extrusionShader = Resources.Load<ComputeShader>(extrusionShaderPath);
            
            _intrusionShader.SetInts("chunk_size", Chunk.Width, Chunk.Height, Chunk.Length);
            _extrusionShader.SetInts("chunk_size", Chunk.Width, Chunk.Height, Chunk.Length);
        }

        public IntrusionResult Intrude(Model model, Vector3 point)
        {
            DateTime before = DateTime.Now;
            
            _intrusionShader.SetInts("model_size", model.Width, model.Height, model.Length);
            _intrusionShader.SetFloat("voxel_size", model.VoxelSize);
            
            _intrusionShader.SetFloat("intrusion_radius", IntrusionStrength / 2f);
            _intrusionShader.SetFloats("intrusion_point", point.x, point.y, point.z);
            
            ComputeBuffer voxelsBuffer = new ComputeBuffer(Chunk.TotalVolume, sizeof(int));

            Chunk[] chunks = (Chunk[]) model.Chunks.Clone();
            
            ModificationShape intrusionShape = new Sphere(point, IntrusionStrength / 2);
            var chunksIdToModify = model.GetInvolvedChunksId(intrusionShape);

            foreach (var id in chunksIdToModify)
            {
                VoxelData[] voxels = (VoxelData[]) model.GetChunkAt(id).Voxels.Clone();
                
                voxelsBuffer.SetData(voxels);
            
                _intrusionShader.SetInts("chunk_id", id.X, id.Y, id.Z);
                _intrusionShader.SetBuffer(0, "voxels", voxelsBuffer);
                
                _intrusionShader.Dispatch(0, 
                    GetGroupsCount(Chunk.Width),
                    GetGroupsCount(Chunk.Height),
                    GetGroupsCount(Chunk.Length));
            
                voxelsBuffer.GetData(voxels);
                chunks[model.GetChunkIndex(id)] = new Chunk(id, voxels);
            }
            
            voxelsBuffer.Dispose();
            
            _logger.Log($"Intrusion took {(DateTime.Now - before).TotalSeconds:f3}");
            
            Model newModel = new Model(chunks, model.Width, model.Height, model.Length, model.VoxelSize);
            return new IntrusionResult(newModel, chunksIdToModify);
        }

        public ExtrusionResult Extrude(Model model, Vector3 point)
        {
            DateTime before = DateTime.Now;
            
            _extrusionShader.SetInts("model_size", model.Width, model.Height, model.Length);
            _extrusionShader.SetFloat("voxel_size", model.VoxelSize);
            
            _extrusionShader.SetFloat("extrusion_radius", ExtrusionStrength / 2f);
            _extrusionShader.SetFloats("extrusion_point", point.x, point.y, point.z);
            
            ComputeBuffer voxelsBuffer = new ComputeBuffer(Chunk.TotalVolume, sizeof(int));

            Chunk[] chunks = (Chunk[]) model.Chunks.Clone();
            
            ModificationShape extrusionShape = new Sphere(point, ExtrusionStrength / 2);
            var chunksIdToModify = model.GetInvolvedChunksId(extrusionShape);
            
            foreach (var id in chunksIdToModify)
            {
                VoxelData[] voxels = (VoxelData[]) model.GetChunkAt(id).Voxels.Clone();
                
                voxelsBuffer.SetData(voxels);
            
                _extrusionShader.SetInts("chunk_id", id.X, id.Y, id.Z);
                _extrusionShader.SetBuffer(0, "voxels", voxelsBuffer);
                
                _extrusionShader.Dispatch(0, 
                    GetGroupsCount(Chunk.Width),
                    GetGroupsCount(Chunk.Height),
                    GetGroupsCount(Chunk.Length));
            
                voxelsBuffer.GetData(voxels);
                chunks[model.GetChunkIndex(id)] = new Chunk(id, voxels);
            }
            
            _logger.Log($"Extrusion took {(DateTime.Now - before).TotalSeconds:f3}");
                
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
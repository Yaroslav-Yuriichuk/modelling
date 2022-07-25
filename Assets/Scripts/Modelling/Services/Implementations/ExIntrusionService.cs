using System;
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
        }

        public Model Intrude(Model model, Vector3 point)
        {
            DateTime before = DateTime.Now;
            
            _intrusionShader.SetInts("model_size", model.Width, model.Height, model.Length);
            _intrusionShader.SetFloat("voxel_size", model.VoxelSize);
            
            _intrusionShader.SetFloat("intrusion_radius", IntrusionStrength / 2f);
            _intrusionShader.SetFloats("intrusion_point", point.x, point.y, point.z);
            
            ComputeBuffer voxelsBuffer = new ComputeBuffer(model.TotalVolume, sizeof(int));

            VoxelData[] voxels = (VoxelData[])model.Voxels.Clone();
            voxelsBuffer.SetData(voxels);
            
            _intrusionShader.SetBuffer(0, "voxels", voxelsBuffer);
            _intrusionShader.Dispatch(0, 
                GetGroupsCount(model.Width),
                GetGroupsCount(model.Height),
                GetGroupsCount(model.Length));
            
            voxelsBuffer.GetData(voxels);
            voxelsBuffer.Dispose();
            
            _logger.Log($"Intrusion took {(DateTime.Now - before).TotalSeconds:f3}");
                
            return new Model(voxels, model.Width, model.Height, model.Length, model.VoxelSize);
        }

        public Model Extrude(Model model, Vector3 point)
        {
            DateTime before = DateTime.Now;
            
            _extrusionShader.SetInts("model_size", model.Width, model.Height, model.Length);
            _extrusionShader.SetFloat("voxel_size", model.VoxelSize);
            
            _extrusionShader.SetFloat("extrusion_radius", ExtrusionStrength / 2f);
            _extrusionShader.SetFloats("extrusion_point", point.x, point.y, point.z);
            
            ComputeBuffer voxelsBuffer = new ComputeBuffer(model.TotalVolume, sizeof(int));

            VoxelData[] voxels = (VoxelData[])model.Voxels.Clone();
            voxelsBuffer.SetData(voxels);
            
            _extrusionShader.SetBuffer(0, "voxels", voxelsBuffer);
            _extrusionShader.Dispatch(0, 
                GetGroupsCount(model.Width),
                GetGroupsCount(model.Height),
                GetGroupsCount(model.Length));
            
            voxelsBuffer.GetData(voxels);
            voxelsBuffer.Dispose();
            
            _logger.Log($"Intrusion took {(DateTime.Now - before).TotalSeconds:f3}");
                
            return new Model(voxels, model.Width, model.Height, model.Length, model.VoxelSize);
        }
        
        private int GetGroupsCount(int modelSize)
        {
            const int threadsInEachDirection = 10;
            return (modelSize + threadsInEachDirection - 1) / threadsInEachDirection;
        }
    }
}
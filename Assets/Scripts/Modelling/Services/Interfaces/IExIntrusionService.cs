using System.Collections.Generic;
using UnityEngine;

namespace Modelling.Services
{
    public struct IntrusionResult
    {
        public Model Model;
        public IEnumerable<ChunkId> ModifiedChunksId;

        public IntrusionResult(Model model, IEnumerable<ChunkId> modifiedChunksId)
        {
            Model = model;
            ModifiedChunksId = modifiedChunksId;
        }
    }

    public struct ExtrusionResult
    {
        public Model Model;
        public IEnumerable<ChunkId> ModifiedChunksId;

        public ExtrusionResult(Model model, IEnumerable<ChunkId> modifiedChunksId)
        {
            Model = model;
            ModifiedChunksId = modifiedChunksId;
        }
    }
    
    public interface IExIntrusionService
    {
        public float IntrusionStrength { get; set; }
        public float ExtrusionStrength { get; set; }
        
        public IntrusionResult Intrude(Model model, Vector3 point);
        public ExtrusionResult Extrude(Model model, Vector3 point);
    }
}
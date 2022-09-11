using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using VoxelModelling.Services;
using ILogger = VoxelModelling.Services.ILogger;

namespace VoxelModelling
{
    public class ModelObject : MonoBehaviour
    {
        public Model Model { get; private set; }

        [SerializeField] private ChunkObject _chunkPrefab;

        private Size3D _sizeInVoxels;
        private Size3D _sizeInChunks;
        private ChunkObject[] _chunkObjects;
        
        private ILogger _logger;
        private IBuildingService _buildingService;
        private IExIntrusionService _exIntrusionService;
        
        [Inject]
        private void Construct(IExIntrusionService exIntrusionService, IBuildingService buildingService, ILogger logger)
        {
            _exIntrusionService = exIntrusionService;
            _buildingService = buildingService;
            _logger = logger;
        }

        public void Init(Size3D size, bool updateCollider)
        {
            _sizeInVoxels = size;
            _sizeInChunks = Chunk.GetSizeInChunks(size);
            
            _chunkObjects = new ChunkObject[_sizeInChunks.TotalVolume];
            
            for (int z = 0; z < _sizeInChunks.Length; z++)
            {
                for (int y = 0; y < _sizeInChunks.Height; y++)
                {
                    for (int x = 0; x < _sizeInChunks.Width; x++)
                    {
                        var chunk = Instantiate(_chunkPrefab, transform);
                        chunk.Init(_buildingService, _logger, updateCollider);
                        _chunkObjects[GetChunkIndex(x, y, z)] = chunk;
                    }
                }
            }
        }

        public IntrusionResult Intrude(Vector3 point)
        {
            DateTime before = DateTime.Now;
            
            var result = _exIntrusionService.Intrude(Model, point);
            ApplyModel(result.Model, result.ModifiedChunksId);
            
            _logger.Log($"Intrusion took {(DateTime.Now - before).TotalSeconds:F3} seconds");
            return result;
        }

        public ExtrusionResult Extrude(Vector3 point)
        {
            DateTime before = DateTime.Now;
            
            var result = _exIntrusionService.Extrude(Model, point);
            ApplyModel(result.Model, result.ModifiedChunksId);
            
            _logger.Log($"Extrusion took {(DateTime.Now - before).TotalSeconds:F3} seconds");
            return result;
        }

        public void ApplyModel(Model model, IEnumerable<ChunkId> сhunksToUpdate = null)
        {
            Model = model;
            
            if (сhunksToUpdate == null)
            {
                for (int i = 0; i < _sizeInChunks.TotalVolume; i++)
                {
                    _chunkObjects[i].ApplyChunk(model.Chunks[i], model);
                }
                return;
            }
            
            foreach (var id in сhunksToUpdate)
            {
                int index = GetChunkIndex(id);
                _chunkObjects[index].ApplyChunk(model.Chunks[index], model);
            }
        }

        private int GetChunkIndex(ChunkId id) => GetChunkIndex(id.X, id.Y, id.Z);

        private int GetChunkIndex(int x, int y, int z)
        {
            return z * _sizeInChunks.Width * _sizeInChunks.Height + y * _sizeInChunks.Width + x;
        }
    }
}
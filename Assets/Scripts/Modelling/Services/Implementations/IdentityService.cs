using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VoxelModelling;

namespace Modelling.Services
{
    public sealed class IdentityService : IIdentityService
    {
        public event Action<float> OnIdentityCalculated;

        private Model _targetModel;

        private int[] _voxelsInPlaceInChunks;
        private int[] _voxelsNotInPlaceInChunks;

        private int _totalVoxelsInPlace;
        private int _totalVoxelsNotInPlace;
        
        public void SetTargetModel(Model targetModel)
        {
            _targetModel = targetModel;
        }

        public async Task Calculate(Model deformableModel, IEnumerable<ChunkId> chunksToRecalculate = null)
        {
            if (deformableModel.Width != _targetModel.Width
                || deformableModel.Height != _targetModel.Height
                || deformableModel.Length != _targetModel.Length) return;

            if (chunksToRecalculate == null)
            {
                _voxelsInPlaceInChunks = new int[deformableModel.SizeInChunks.TotalVolume];
                _voxelsNotInPlaceInChunks = new int[deformableModel.SizeInChunks.TotalVolume];
                
                _totalVoxelsInPlace = _totalVoxelsNotInPlace = 0;

                await Task.Run(() =>
                {
                    for (int i = 0; i < deformableModel.SizeInChunks.TotalVolume; i++)
                    {
                        (int voxelsInPlace , int voxelsNotInPlace) =
                            CalculateChunk(deformableModel.Chunks[i], _targetModel.Chunks[i]);

                        _voxelsInPlaceInChunks[i] = voxelsInPlace;
                        _voxelsNotInPlaceInChunks[i] = voxelsNotInPlace;
                    
                        _totalVoxelsInPlace += voxelsInPlace;
                        _totalVoxelsNotInPlace += voxelsNotInPlace;
                    }
                });

                OnIdentityCalculated?.Invoke((float)_totalVoxelsInPlace / (_totalVoxelsInPlace + _totalVoxelsNotInPlace));
                return;
            }

            await Task.Run(() =>
            {
                foreach (var id in chunksToRecalculate)
                {
                    int index = deformableModel.GetChunkIndex(id);

                    (int voxelsInPlace, int voxelsNotInPlace) =
                        CalculateChunk(deformableModel.Chunks[index], _targetModel.Chunks[index]);

                    _totalVoxelsInPlace -= _voxelsInPlaceInChunks[index];
                    _totalVoxelsNotInPlace -= _voxelsNotInPlaceInChunks[index];

                    _voxelsInPlaceInChunks[index] = voxelsInPlace;
                    _voxelsNotInPlaceInChunks[index] = voxelsNotInPlace;

                    _totalVoxelsInPlace += voxelsInPlace;
                    _totalVoxelsNotInPlace += voxelsNotInPlace;
                }
            });

            OnIdentityCalculated?.Invoke((float)_totalVoxelsInPlace / (_totalVoxelsInPlace + _totalVoxelsNotInPlace));
        }

        private (int, int) CalculateChunk(Chunk deformableModelChunk, Chunk targetModelChunk)
        {
            int voxelsInPlace = 0, voxelsNotInPlace = 0;
            VoxelData tmpVoxel1, tmpVoxel2;
            
            for (int x = 0; x < Chunk.Width; x++)
            {
                for (int y = 0; y < Chunk.Height; y++)
                {
                    for (int z = 0; z < Chunk.Length; z++)
                    {
                        tmpVoxel1 = deformableModelChunk.GetVoxelAt(x, y, z);
                        tmpVoxel2 = targetModelChunk.GetVoxelAt(x, y, z);
                        
                        voxelsInPlace += tmpVoxel1.Exists & tmpVoxel2.Exists;
                        voxelsNotInPlace += (tmpVoxel1.Exists + tmpVoxel2.Exists) & 1;
                    }
                }
            }

            return (voxelsInPlace, voxelsNotInPlace);
        }
    }
}
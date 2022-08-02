using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Modelling.Services
{
    public interface IIdentityService
    {
        public event Action<float> OnIdentityCalculated;

        public void SetTargetModel(Model targetModel);
        public Task Calculate(Model deformableModel, IEnumerable<ChunkId> chunksToRecalculate = null);
    }
}
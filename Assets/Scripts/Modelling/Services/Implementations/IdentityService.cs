using System;
using UnityEngine;

namespace Modelling.Services
{
    public class IdentityService : IIdentityService
    {
        public event Action<float> OnIdentityCalculated;
        
        public void Calculate(Model firstModel, Model secondModel)
        {
            if (firstModel.Width != secondModel.Width
                || firstModel.Height != secondModel.Height || firstModel.Length != secondModel.Length) return;

            int voxelsInPlace = 0;
            int voxelsNotInPlace = 0;

            VoxelData tmpVoxel1, tmpVoxel2;
            
            for (int x = 0; x < firstModel.Width; x++)
            {
                for (int y = 0; y < firstModel.Height; y++)
                {
                    for (int z = 0; z < firstModel.Length; z++)
                    {
                        tmpVoxel1 = firstModel.GetVoxelAt(x, y, z);
                        tmpVoxel2 = secondModel.GetVoxelAt(x, y, z);
                        
                        voxelsInPlace += tmpVoxel1.Exists & tmpVoxel2.Exists;
                        voxelsNotInPlace += (tmpVoxel1.Exists + tmpVoxel2.Exists) & 1;
                    }
                }
            }
            
            OnIdentityCalculated?.Invoke((float)voxelsInPlace / (voxelsInPlace + voxelsNotInPlace));
        }
    }
}
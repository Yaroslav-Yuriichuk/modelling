﻿using UnityEngine;

namespace Modelling.Geometry
{
    public abstract class ModificationShape
    {
        public abstract void MoveBy(Vector3 offset);
        public abstract bool Intersects(Chunk chunk, float voxelSize);
    }
}
using System;

namespace VoxelModelling.Attributes
{
    public class ConstructableShapeAttribute : Attribute
    {
        public Type Type { get; private set; }

        public ConstructableShapeAttribute(Type type)
        {
            Type = type;
        }
    }
}
using System;
using System.Reflection;
using Modelling.Attributes;
using Modelling.Geometry;
using Modelling.Services;
using UnityEngine;

namespace Modelling.Extensions
{
    public static class ModificationShapeExtensions
    {
        public static ModificationShape Construct(this ShapeType shapeType, Vector3 pivot, float size)
        {
            ModificationShape shape = null;
            Type type = GetType(shapeType);

            if (type is {IsAbstract: false})
            {
                shape = (ModificationShape) Activator.CreateInstance(type, pivot, size);
            }
            
            return shape;
        }

        private static Type GetType(ShapeType shapeType)
        {
            FieldInfo fieldInfo = typeof(ShapeType).GetField(Enum.GetName(typeof(ShapeType), shapeType));
            ConstructableShapeAttribute attribute = (ConstructableShapeAttribute)
                Attribute.GetCustomAttribute(fieldInfo, typeof(ConstructableShapeAttribute));

            return attribute.Type;
        }
    }
}
using System;
using UnityEngine;
using VoxelModelling.Services;

namespace Modelling.ScriptableObjects
{
    [Serializable]
    public struct ShapeData
    {
        public string Name;
        public ShapeType ShapeType;
    }
    
    [CreateAssetMenu(fileName = "ShapeOptions", menuName = "Modelling/ShapeOptions")]
    public class ShapeOptions : ScriptableObject
    {
        public ShapeData[] ShapeData;
    }
}
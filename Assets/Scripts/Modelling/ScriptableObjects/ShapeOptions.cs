using System;
using Modelling.Services;
using UnityEngine;

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
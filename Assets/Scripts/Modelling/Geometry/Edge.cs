using UnityEngine;

namespace Modelling.Geometry
{
    public struct Edge
    {
        public Vector3 A;
        public Vector3 B;

        public Edge(Vector3 a, Vector3 b)
        {
            A = a;
            B = b;
        }
    }
}
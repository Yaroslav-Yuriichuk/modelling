using UnityEngine;

namespace Modelling.Geometry
{
    public class Sphere : ModificationShape
    {
        private Vector3 _center;
        private float _radius;

        public Sphere(Vector3 center, float radius)
        {
            _center = center;
            _radius = radius;
        }

        public override void MoveBy(Vector3 offset)
        {
            _center += offset;
        }

        public override bool Intersects(Chunk chunk, float voxelSize)
        {
            Vector3 chunkOffset = new Vector3(
                voxelSize * chunk.Id.X * Chunk.Width,
                voxelSize * chunk.Id.Y * Chunk.Height,
                voxelSize * chunk.Id.Z * Chunk.Length);

            return IntersectsWithSphereEdgePoints(voxelSize, chunkOffset)
                || IntersectsWithEdges(voxelSize, chunkOffset);
        }

        private bool IntersectsWithEdge(Edge edge)
        {
            if (IsInSphere(edge.A) || IsInSphere(edge.B)) return true;

            Vector3 projection = Vector3.Project(_center - edge.A, edge.B - edge.A);
            Vector3 closestPointToCenter = edge.A + projection;

            if (Vector3.Distance(closestPointToCenter, _center) > _radius) return false;

            return Vector3.Dot(closestPointToCenter - edge.A, edge.B - edge.A) > 0
                && Vector3.Dot(closestPointToCenter - edge.B, edge.A - edge.B) > 0;
        }

        private bool IsInSphere(Vector3 point)
        {
            return Vector3.Distance(_center, point) <= _radius;
        }

        private bool IntersectsWithSphereEdgePoints(float voxelSize, Vector3 chunkOffset)
        {
            bool IsPointInChunk(Vector3 point)
            {
                bool isInside;

                isInside = (chunkOffset.x - point.x) * (chunkOffset.x + voxelSize * Chunk.Width - point.x) <= 0;
                isInside = isInside && (chunkOffset.y - point.y) * (chunkOffset.y + voxelSize * Chunk.Height - point.y) <= 0;
                isInside = isInside && (chunkOffset.z - point.z) * (chunkOffset.z + voxelSize * Chunk.Length - point.z) <= 0;
            
                return isInside;
            }

            return IsPointInChunk(_center)
                   || IsPointInChunk(_center + _radius * Vector3.forward)
                   || IsPointInChunk(_center + _radius * Vector3.back)
                   || IsPointInChunk(_center + _radius * Vector3.left)
                   || IsPointInChunk(_center + _radius * Vector3.right)
                   || IsPointInChunk(_center + _radius * Vector3.up)
                   || IsPointInChunk(_center + _radius * Vector3.down);
        }

        private bool IntersectsWithEdges(float voxelSize, Vector3 chunkOffset)
        {
            Edge[] edges =
            {
                new Edge(chunkOffset, chunkOffset + voxelSize * new Vector3(Chunk.Width, 0, 0)),
                
                new Edge(chunkOffset + voxelSize * new Vector3(0, Chunk.Height, 0),
                    chunkOffset + voxelSize * new Vector3(Chunk.Width, Chunk.Height, 0)),
                
                new Edge(chunkOffset + voxelSize * new Vector3(0, 0, Chunk.Length),
                    chunkOffset + voxelSize * new Vector3(Chunk.Width, 0, Chunk.Length)),
                
                new Edge(chunkOffset + voxelSize * new Vector3(0, Chunk.Height, Chunk.Length),
                    chunkOffset + voxelSize * new Vector3(Chunk.Width, Chunk.Height, Chunk.Length)),
                
                new Edge(chunkOffset, chunkOffset + voxelSize * new Vector3(0, Chunk.Height, 0)),
                
                new Edge(chunkOffset + voxelSize * new Vector3(Chunk.Width, 0, 0),
                    chunkOffset + voxelSize * new Vector3(Chunk.Width, Chunk.Height, 0)),
                
                new Edge(chunkOffset + voxelSize * new Vector3(0, 0, Chunk.Length),
                    chunkOffset + voxelSize * new Vector3(0, Chunk.Height, Chunk.Length)),
                
                new Edge(chunkOffset + voxelSize * new Vector3(Chunk.Width, 0, Chunk.Length),
                    chunkOffset + voxelSize * new Vector3(Chunk.Width, Chunk.Height, Chunk.Length)),
                
                new Edge(chunkOffset, chunkOffset + voxelSize * new Vector3(0, 0, Chunk.Length)),
                
                new Edge(chunkOffset + voxelSize * new Vector3(Chunk.Width, 0, 0),
                    chunkOffset + voxelSize * new Vector3(Chunk.Width, 0, Chunk.Length)),
                
                new Edge(chunkOffset + voxelSize * new Vector3(0, Chunk.Height, 0),
                    chunkOffset + voxelSize * new Vector3(0, Chunk.Height, Chunk.Length)),
                
                new Edge(chunkOffset + voxelSize * new Vector3(Chunk.Width, Chunk.Height, 0),
                    chunkOffset + voxelSize * new Vector3(Chunk.Width, Chunk.Height, Chunk.Length)),
            };

            foreach (var edge in edges)
            {
                if (IntersectsWithEdge(edge))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
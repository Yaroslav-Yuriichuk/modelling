using UnityEngine;
using UnityEngine.Rendering;

namespace Modelling.Services
{
    public class BuildingService : IBuildingService
    {
        private struct VoxelTrianglesData
        {
            public int XFrontSideIndex;
            public int XBackSideIndex;
            public int YFrontSideIndex;
            public int YBackSideIndex;
            public int ZFrontSideIndex;
            public int ZBackSideIndex;

            public VoxelTrianglesData(int xFrontSideIndex, int xBackSideIndex, int yFrontSideIndex,
                int yBackSideIndex, int zFrontSideIndex, int zBackSideIndex)
            {
                XFrontSideIndex = xFrontSideIndex;
                XBackSideIndex = xBackSideIndex;
                YFrontSideIndex = yFrontSideIndex;
                YBackSideIndex = yBackSideIndex;
                ZFrontSideIndex = zFrontSideIndex;
                ZBackSideIndex = zBackSideIndex;
            }
        }
        
        private readonly ILogger _logger;
        private readonly ComputeShader _chunkBuildingShader;

        private readonly VoxelTrianglesData[] _data;
        private readonly ComputeBuffer _voxelTrianglesDataBuffer;
        
        public BuildingService(ILogger logger)
        {
            _logger = logger;
            
            const string path = "Compute Shaders/Building/ChunkBuildingShader";
            _chunkBuildingShader = Resources.Load<ComputeShader>(path);
            
            _data = new VoxelTrianglesData[Chunk.TotalVolume];
            _voxelTrianglesDataBuffer = new ComputeBuffer(Chunk.TotalVolume, 6 * sizeof(int));
            
            _chunkBuildingShader.SetInts("chunk_size", Chunk.Width, Chunk.Height, Chunk.Length);
            _chunkBuildingShader.SetBuffer(0, "voxel_triangles_data", _voxelTrianglesDataBuffer);
        }

        ~BuildingService()
        {
            _voxelTrianglesDataBuffer.Dispose();
        }
        
        public ChunkBuildResult Build(Chunk chunk, Model model)
        {
            if (model == null || chunk == null) return new ChunkBuildResult(false, null);

            int trianglesCount = DetermineTrianglesCount(chunk);

            if (trianglesCount == 0) return new ChunkBuildResult(true, null);
            
            ComputeBuffer verticesBuffer = new ComputeBuffer(trianglesCount * 2, 3 * sizeof(float));
            ComputeBuffer trianglesBuffer = new ComputeBuffer(trianglesCount * 3, sizeof(int));
            
            Vector3[] vertices = new Vector3[trianglesCount * 2];
            int[] triangles = new int[trianglesCount * 3];
            
            _voxelTrianglesDataBuffer.SetData(_data);

            _chunkBuildingShader.SetInts("model_size", model.Width, model.Height, model.Length);
            _chunkBuildingShader.SetInts("chunk_id", chunk.Id.X, chunk.Id.Y, chunk.Id.Z);
            _chunkBuildingShader.SetFloat("voxel_size", model.VoxelSize);
            
            _chunkBuildingShader.SetBuffer(0, "vertices", verticesBuffer);
            _chunkBuildingShader.SetBuffer(0, "triangles", trianglesBuffer);

            _chunkBuildingShader.Dispatch(0, 
                GetGroupsCount(Chunk.Width),
                GetGroupsCount(Chunk.Height),
                GetGroupsCount(Chunk.Length));

            verticesBuffer.GetData(vertices);
            trianglesBuffer.GetData(triangles);
            
            Mesh mesh = new Mesh
            {
                indexFormat = IndexFormat.UInt32,
                vertices = vertices,
                triangles = triangles
            };
            
            mesh.RecalculateNormals();
            
            verticesBuffer.Dispose();
            trianglesBuffer.Dispose();

            return new ChunkBuildResult(true, mesh);
        }
        
        private int GetGroupsCount(int modelSize)
        {
            const int threadsInEachDirection = 10;
            return (modelSize + threadsInEachDirection - 1) / threadsInEachDirection;
        }

        private int DetermineTrianglesCount(Chunk chunk)
        {
            for (int i = 0; i < Chunk.TotalVolume; i++)
            {
                _data[i] = new VoxelTrianglesData(-1, -1, -1, -1, -1, -1);
            }
            
            int numberOfSides = 0;
            VoxelData tmpVoxel;
            bool tmp;
            
            bool HaveDisplayedSide(VoxelData voxel1, VoxelData voxel2, out bool faceFromFirstVoxel)
            {
                faceFromFirstVoxel = voxel1.Exists == 1;
                return ((voxel1.Exists + voxel2.Exists) & 1) == 1;
            }

            for (int y = 0; y < Chunk.Height; y++)
            {
                for (int z = 0; z < Chunk.Length; z++)
                {
                    tmpVoxel = chunk.GetVoxelAt(0, y, z);
                    if (tmpVoxel.Exists == 1)
                    {
                        _data[chunk.GetVoxelIndex(0, y, z)].XBackSideIndex = numberOfSides++;
                    }
                }
            }
            
            for (int x = 0; x < Chunk.Width; x++)
            {
                for (int y = 0; y < Chunk.Height; y++)
                {
                    tmpVoxel = chunk.GetVoxelAt(x, y, 0);
                    if (tmpVoxel.Exists == 1)
                    {
                        _data[chunk.GetVoxelIndex(x, y, 0)].ZBackSideIndex = numberOfSides++;
                    }
                }
            }

            for (int x = 0; x < Chunk.Width; x++)
            {
                for (int z = 0; z < Chunk.Length; z++)
                {
                    tmpVoxel = chunk.GetVoxelAt(x, 0, z);
                    if (tmpVoxel.Exists == 1)
                    {
                        _data[chunk.GetVoxelIndex(x, 0, z)].YBackSideIndex = numberOfSides++;
                    }
                }
            }

            for (int x = 0; x < Chunk.Width; x++)
            {
                for (int y = 0; y < Chunk.Height; y++)
                {
                    for (int z = 0; z < Chunk.Length; z++)
                    {
                        tmpVoxel = chunk.GetVoxelAt(x, y, z);

                        if (HaveDisplayedSide(tmpVoxel, chunk.GetVoxelAt(x, y, z + 1), out tmp))
                        {
                            if (tmp)
                            {
                                _data[chunk.GetVoxelIndex(x, y, z)].ZFrontSideIndex = numberOfSides++;   
                            }
                            else
                            {
                                _data[chunk.GetVoxelIndex(x, y, z + 1)].ZBackSideIndex = numberOfSides++;
                            }
                        }

                        if (HaveDisplayedSide(tmpVoxel, chunk.GetVoxelAt(x, y + 1, z), out tmp))
                        {
                            if (tmp)
                            {
                                _data[chunk.GetVoxelIndex(x, y, z)].YFrontSideIndex = numberOfSides++;    
                            }
                            else
                            {
                                _data[chunk.GetVoxelIndex(x, y + 1, z)].YBackSideIndex = numberOfSides++;
                            }
                        }

                        if (HaveDisplayedSide(tmpVoxel, chunk.GetVoxelAt(x + 1, y, z), out tmp))
                        {
                            if (tmp)
                            {
                                _data[chunk.GetVoxelIndex(x, y, z)].XFrontSideIndex = numberOfSides++;   
                            }
                            else
                            {
                                _data[chunk.GetVoxelIndex(x + 1, y, z)].XBackSideIndex = numberOfSides++;
                            }
                        }
                    }
                }
            }
            
            return numberOfSides * 2;
        }
    }
}
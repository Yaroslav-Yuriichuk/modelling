using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace Modelling.Services
{
    public class ModelBuildingServiceCPUGPU : IModelBuildingService
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
        
        private ComputeShader _modelBuildingShader;

        private readonly ILogger _logger;
        
        public ModelBuildingServiceCPUGPU(ILogger logger)
        {
            _logger = logger;
            
            const string path = "Compute Shaders/Building/ModelBuildingShader";
            _modelBuildingShader = Resources.Load<ComputeShader>(path);
        }

        public Mesh Build(Model model)
        {
            if (model == null) return null;
            
            _logger.Log("Started building");
            DateTime before = DateTime.Now;
            
            VoxelTrianglesData[] data = new VoxelTrianglesData[model.TotalVolume];
            int trianglesCount = DetermineTrianglesCount(model, data);
            _logger.Log($"Total triangles: {trianglesCount}");

            if (trianglesCount == 0) return null;
            
            ComputeBuffer voxelTrianglesDataBuffer = new ComputeBuffer(model.TotalVolume, 6 * sizeof(int));
            ComputeBuffer verticesBuffer = new ComputeBuffer(trianglesCount * 2, 3 * sizeof(float));
            ComputeBuffer trianglesBuffer = new ComputeBuffer(trianglesCount * 3, sizeof(int));
            
            Vector3[] vertices = new Vector3[trianglesCount * 2];
            int[] triangles = new int[trianglesCount * 3];
            
            voxelTrianglesDataBuffer.SetData(data);

            _modelBuildingShader.SetInts("model_size", model.Width, model.Height, model.Length);
            _modelBuildingShader.SetFloat("voxel_size", model.VoxelSize);
            
            _modelBuildingShader.SetBuffer(0, "voxel_triangles_data", voxelTrianglesDataBuffer);
            _modelBuildingShader.SetBuffer(0, "vertices", verticesBuffer);
            _modelBuildingShader.SetBuffer(0, "triangles", trianglesBuffer);

            _modelBuildingShader.Dispatch(0, 
                GetGroupsCount(model.Width),
                GetGroupsCount(model.Height),
                GetGroupsCount(model.Length));

            verticesBuffer.GetData(vertices);
            trianglesBuffer.GetData(triangles);
            
            Mesh mesh = new Mesh
            {
                indexFormat = IndexFormat.UInt32,
                vertices = vertices,
                triangles = triangles
            };

            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
            
            _logger.Log($"Building took {(DateTime.Now - before).TotalSeconds:f3} seconds");
            
            voxelTrianglesDataBuffer.Dispose();
            verticesBuffer.Dispose();
            trianglesBuffer.Dispose();
            
            return mesh;
        }
        
        private int GetGroupsCount(int modelSize)
        {
            const int threadsInEachDirection = 10;
            return (modelSize + threadsInEachDirection - 1) / threadsInEachDirection;
        }

        private int DetermineTrianglesCount(Model model, VoxelTrianglesData[] data)
        {
            for (int i = 0; i < model.TotalVolume; i++)
            {
                data[i] = new VoxelTrianglesData(-1, -1, -1, -1, -1, -1);
            }
            
            int numberOfSides = 0;
            VoxelData tmpVoxel;
            bool tmp;
            
            bool HaveDisplayedSide(VoxelData voxel1, VoxelData voxel2, out bool faceFromFirstVoxel)
            {
                faceFromFirstVoxel = voxel1.Exists == 1;
                return ((voxel1.Exists + voxel2.Exists) & 1) == 1;
            }

            for (int y = 0; y < model.Height; y++)
            {
                for (int z = 0; z < model.Length; z++)
                {
                    tmpVoxel = model.GetVoxelAt(0, y, z);
                    if (tmpVoxel.Exists == 1)
                    {
                        data[model.GetVoxelIndex(0, y, z)].XBackSideIndex = numberOfSides++;
                    }
                }
            }
            
            for (int x = 0; x < model.Width; x++)
            {
                for (int y = 0; y < model.Height; y++)
                {
                    tmpVoxel = model.GetVoxelAt(x, y, 0);
                    if (tmpVoxel.Exists == 1)
                    {
                        data[model.GetVoxelIndex(x, y, 0)].ZBackSideIndex = numberOfSides++;
                    }
                }
            }

            for (int x = 0; x < model.Width; x++)
            {
                for (int z = 0; z < model.Length; z++)
                {
                    tmpVoxel = model.GetVoxelAt(x, 0, z);
                    if (tmpVoxel.Exists == 1)
                    {
                        data[model.GetVoxelIndex(x, 0, z)].YBackSideIndex = numberOfSides++;
                    }
                }
            }

            for (int x = 0; x < model.Width; x++)
            {
                for (int y = 0; y < model.Height; y++)
                {
                    for (int z = 0; z < model.Length; z++)
                    {
                        tmpVoxel = model.GetVoxelAt(x, y, z);

                        if (HaveDisplayedSide(tmpVoxel, model.GetVoxelAt(x, y, z + 1), out tmp))
                        {
                            if (tmp)
                            {
                                data[model.GetVoxelIndex(x, y, z)].ZFrontSideIndex = numberOfSides++;   
                            }
                            else
                            {
                                data[model.GetVoxelIndex(x, y, z + 1)].ZBackSideIndex = numberOfSides++;
                            }
                        }

                        if (HaveDisplayedSide(tmpVoxel, model.GetVoxelAt(x, y + 1, z), out tmp))
                        {
                            if (tmp)
                            {
                                data[model.GetVoxelIndex(x, y, z)].YFrontSideIndex = numberOfSides++;    
                            }
                            else
                            {
                                data[model.GetVoxelIndex(x, y + 1, z)].YBackSideIndex = numberOfSides++;
                            }
                        }

                        if (HaveDisplayedSide(tmpVoxel, model.GetVoxelAt(x + 1, y, z), out tmp))
                        {
                            if (tmp)
                            {
                                data[model.GetVoxelIndex(x, y, z)].XFrontSideIndex = numberOfSides++;   
                            }
                            else
                            {
                                data[model.GetVoxelIndex(x + 1, y, z)].XBackSideIndex = numberOfSides++;
                            }
                        }
                    }
                }
            }
            
            return numberOfSides * 2;
        }
    }
}
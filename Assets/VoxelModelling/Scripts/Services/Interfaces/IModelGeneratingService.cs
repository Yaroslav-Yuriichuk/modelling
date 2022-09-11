namespace VoxelModelling.Services
{
    public enum ModelType
    {
        Cube,
        Sphere,
        FlattenedSphere,
    }
    
    public struct ModelConstraints
    {
        public ModelType Type;
        
        public int Width;  // X
        public int Height; // Y
        public int Length; // Z

        public float VoxelSize;

        public Size3D Size => new Size3D(Width, Height, Length);
        
        public ModelConstraints(ModelType type, int width, int height, int length, float voxelSize)
        {
            Type = type;
            
            Width = width;
            Height = height;
            Length = length;

            VoxelSize = voxelSize;
        }
    }
    
    public interface IModelGeneratingService
    {
        public Model GenerateModel(ModelConstraints constraints);
    }
}

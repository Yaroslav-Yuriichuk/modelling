namespace Modelling.Services
{
    public enum SoundType
    {
        Intrusion,
        Extrusion,
    }
    
    public interface IAudioService
    {
        public void PlaySound(SoundType type);
    }
}
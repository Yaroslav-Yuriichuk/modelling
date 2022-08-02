namespace Modelling.Services
{
    public interface ILoadingService
    {
        public bool IsLoading { get; }
        
        public void Show();
        public void Hide();
    }
}
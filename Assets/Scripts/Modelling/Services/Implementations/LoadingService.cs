using Modelling.UI;

namespace Modelling.Services
{
    public sealed class LoadingService : ILoadingService
    {
        public bool IsLoading { get; private set; }

        private readonly LoadingElement _loadingElement;
        
        public LoadingService(LoadingElement loadingElement)
        {
            _loadingElement = loadingElement;
        }
        
        public void Show()
        {
            IsLoading = true;
            _loadingElement.Show();
        }

        public void Hide()
        {
            _loadingElement.Hide(() => IsLoading = false);
        }
    }
}
using Modelling.UI;

namespace Modelling.Services
{
    public class LoadingService : ILoadingService
    {
        public bool IsLoading { get; private set; }

        private Loading _loading;
        
        public LoadingService(Loading loading)
        {
            _loading = loading;
        }
        
        public void Show()
        {
            IsLoading = true;
            _loading.Show();
        }

        public void Hide()
        {
            IsLoading = false;
            _loading.Hide();
        }
    }
}
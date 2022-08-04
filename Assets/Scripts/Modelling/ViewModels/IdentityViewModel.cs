using Modelling.Services;
using UnityWeld.Binding;
using Zenject;

namespace Modelling.ViewModels
{
    [Binding]
    public class IdentityViewModel : MonoBehaviourViewModel
    {
        #region Binding properties

        private float _identity;

        [Binding]
        public float Identity
        {
            get => _identity;
            set
            {
                if (_identity == value)
                    return;

                _identity = value;
                OnPropertyChanged();
            }
        }

        #endregion
        
        private IIdentityService _identityService;
        
        [Inject]
        private void Construct(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        private void OnEnable()
        {
            _identityService.OnIdentityCalculated += IdentityCalculatedEventHandler;
        }

        private void OnDisable()
        {
            _identityService.OnIdentityCalculated -= IdentityCalculatedEventHandler;
        }
        
        private void IdentityCalculatedEventHandler(float identity)
        {
            Identity = identity * 100f;
        }
    }
}
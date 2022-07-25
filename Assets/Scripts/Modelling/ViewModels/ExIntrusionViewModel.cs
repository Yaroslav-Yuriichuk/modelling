using System;
using Modelling.Services;
using UnityWeld.Binding;
using Zenject;

namespace Modelling.ViewModels
{
    [Binding]
    public class ExIntrusionViewModel : MonoBehaviourViewModel
    {
        #region Binding properties

        private bool _isIntrusionSelected = true;
        
        [Binding]
        public bool IsIntrusionSelected
        {
            get => _isIntrusionSelected;
            set
            {
                if (_isIntrusionSelected == value)
                    return;

                _isIntrusionSelected = value;
                OnPropertyChanged();
            }
        }
        
        private bool _isExtrusionSelected = false;

        [Binding]
        public bool IsExtrusionSelected
        {
            get => _isExtrusionSelected;
            set
            {
                if (_isExtrusionSelected == value)
                    return;

                _isExtrusionSelected = value;
                OnPropertyChanged();
            }
        }

        private float _intrusionStrength = 0.2f;

        [Binding]
        public float IntrusionStrength
        {
            get => _intrusionStrength;
            set
            {
                if (_intrusionStrength == value)
                    return;

                _exIntrusionService.IntrusionStrength = value;
                _intrusionStrength = value;
                OnPropertyChanged();
            }
        }
        
        private float _extrusionStrength = 0.3f;

        [Binding]
        public float ExtrusionStrength
        {
            get => _extrusionStrength;
            set
            {
                if (_extrusionStrength == value)
                    return;

                _exIntrusionService.ExtrusionStrength = value;
                _extrusionStrength = value;
                OnPropertyChanged();
            }
        }

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

        private IExIntrusionService _exIntrusionService;
        private IIdentityService _identityService;
        
        [Inject]
        private void Construct(IExIntrusionService exIntrusionService, IIdentityService identityService)
        {
            _exIntrusionService = exIntrusionService;
            _identityService = identityService;
            
            IntrusionStrength = _exIntrusionService.IntrusionStrength;
            ExtrusionStrength = _exIntrusionService.ExtrusionStrength;
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
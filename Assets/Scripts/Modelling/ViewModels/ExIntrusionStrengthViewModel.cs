using UnityWeld.Binding;
using VoxelModelling.Services;
using Zenject;

namespace Modelling.ViewModels
{
    [Binding]
    public sealed class ExIntrusionStrengthViewModel : MonoBehaviourViewModel
    {
        #region Binding properties

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

        #endregion
        
        private IExIntrusionService _exIntrusionService;
        
        [Inject]
        private void Construct(IExIntrusionService exIntrusionService)
        {
            _exIntrusionService = exIntrusionService;
            
            IntrusionStrength = _exIntrusionService.IntrusionStrength;
            ExtrusionStrength = _exIntrusionService.ExtrusionStrength;
        }
    }
}
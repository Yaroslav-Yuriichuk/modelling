using UnityWeld.Binding;

namespace Modelling.ViewModels
{
    [Binding]
    public sealed class ExIntrusionViewModel : MonoBehaviourViewModel
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

        #endregion
    }
}
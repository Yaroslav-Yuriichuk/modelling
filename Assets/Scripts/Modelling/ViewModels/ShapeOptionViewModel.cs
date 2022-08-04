using System;
using Modelling.ScriptableObjects;
using Modelling.Services;
using UnityEngine.UI;
using UnityWeld.Binding;

namespace Modelling.ViewModels
{
    [Binding]
    public class ShapeOptionViewModel : NonMonoBehaviourViewModel
    {
        #region Binding properties

        private string _name;

        [Binding]
        public string Name
        {
            get => _name;
            set
            {
                if (_name == value)
                    return;

                _name = value;
                OnPropertyChanged();
            }
        }

        private bool _isSelected;

        [Binding]
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected == value)
                    return;

                _isSelected = value;
                OnPropertyChanged();

                if (_isSelected)
                {
                    _onShapeSelected?.Invoke(_data.ShapeType);
                }
            }
        }

        private ToggleGroup _toggleGroup;

        [Binding]
        public ToggleGroup ToggleGroup
        {
            get => _toggleGroup;
            set
            {
                if (_toggleGroup == value)
                    return;

                _toggleGroup = value;
                OnPropertyChanged();
            }
        }
        
        #endregion
        
        private readonly ShapeSelectionViewModel _shapeSelectionViewModel;
        private readonly ShapeData _data;
        private readonly Action<ShapeType> _onShapeSelected;

        public ShapeOptionViewModel(ShapeSelectionViewModel shapeSelectionViewModel, ShapeData data,
            Action<ShapeType> onShapeSelected)
        {
            _shapeSelectionViewModel = shapeSelectionViewModel;
            _data = data;
            _onShapeSelected = onShapeSelected;

            ToggleGroup = _shapeSelectionViewModel.ToggleGroup;
            Name = _data.Name;
        }
    }
}
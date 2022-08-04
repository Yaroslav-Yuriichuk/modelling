using Modelling.ScriptableObjects;
using Modelling.Services;
using UnityEngine;
using UnityEngine.UI;
using UnityWeld.Binding;
using Zenject;

namespace Modelling.ViewModels
{
    [Binding]
    public abstract class ShapeSelectionViewModel : MonoBehaviourViewModel
    {
        [SerializeField] private ShapeOptions _shapeOptions;
        [SerializeField] private ToggleGroup _toggleGroup;

        #region Binding properties

        [Binding]
        public ObservableList<ShapeOptionViewModel> ShapeOptions { get; } = new ObservableList<ShapeOptionViewModel>();

        #endregion

        public ToggleGroup ToggleGroup => _toggleGroup;

        protected IExIntrusionService _exIntrusionService;
        
        [Inject]
        private void Construct(IExIntrusionService exIntrusionService)
        {
            _exIntrusionService = exIntrusionService;
        }
        
        private void Start()
        {
            foreach (var option in _shapeOptions.ShapeData)
            {
                ShapeOptions.Add(new ShapeOptionViewModel(this, option, OnShapeTypeSelected));
            }
        }

        protected abstract void OnShapeTypeSelected(ShapeType shapeType);
    }
}
using Modelling.Services;
using UnityWeld.Binding;

namespace Modelling.ViewModels
{
    [Binding]
    public class ExtrusionShapeSelectionViewModel : ShapeSelectionViewModel
    {
        protected override void OnShapeTypeSelected(ShapeType shapeType)
        {
            _exIntrusionService.ExtrusionShapeType = shapeType;
        }
    }
}
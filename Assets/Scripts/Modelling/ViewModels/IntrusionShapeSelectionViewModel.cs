using Modelling.Services;
using UnityWeld.Binding;

namespace Modelling.ViewModels
{
    [Binding]
    public class IntrusionShapeSelectionViewModel : ShapeSelectionViewModel
    {
        protected override void OnShapeTypeSelected(ShapeType shapeType)
        {
            _exIntrusionService.IntrusionShapeType = shapeType;
        }
    }
}
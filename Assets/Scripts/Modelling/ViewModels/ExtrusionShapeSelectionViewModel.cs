using UnityWeld.Binding;
using VoxelModelling.Services;

namespace Modelling.ViewModels
{
    [Binding]
    public sealed class ExtrusionShapeSelectionViewModel : ShapeSelectionViewModel
    {
        protected override void OnShapeTypeSelected(ShapeType shapeType)
        {
            _exIntrusionService.ExtrusionShapeType = shapeType;
        }
    }
}
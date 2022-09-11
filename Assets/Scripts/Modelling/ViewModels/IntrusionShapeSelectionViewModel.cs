﻿using UnityWeld.Binding;
using VoxelModelling.Services;

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
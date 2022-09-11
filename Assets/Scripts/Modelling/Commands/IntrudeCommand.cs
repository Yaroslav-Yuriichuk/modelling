﻿using Modelling.Services;
using UnityEngine;
using VoxelModelling;
using VoxelModelling.Services;

namespace Modelling.Commands
{
    public class IntrudeCommand : ICommand
    {
        private readonly ModelObject _modelObject;
        private readonly Vector3 _point;
        private readonly IIdentityService _identityService;

        private Model _previousModel;
        private IntrusionResult _intrusionResult;

        public IntrudeCommand(ModelObject modelObject, Vector3 point, IIdentityService identityService)
        {
            _modelObject = modelObject;
            _point = point;
            _identityService = identityService;
        }
        
        public void Execute()
        {
            _previousModel = _modelObject.Model;
            _intrusionResult = _modelObject.Intrude(_point);
            _identityService.Calculate(_modelObject.Model, _intrusionResult.ModifiedChunksId);
        }

        public void Undo()
        {
            _modelObject.ApplyModel(_previousModel, _intrusionResult.ModifiedChunksId);
            _identityService.Calculate(_modelObject.Model, _intrusionResult.ModifiedChunksId);
        }
    }
}
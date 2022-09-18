using Modelling.Services;
using UnityEngine;
using VoxelModelling;
using VoxelModelling.Services;

namespace Modelling.Commands
{
    public sealed class ExtrudeCommand : ICommand
    {
        private readonly ModelObject _modelObject;
        private readonly Vector3 _point;
        private readonly IIdentityService _identityService;

        private Model _previousModel;
        private ExtrusionResult _extrusionResult;

        public ExtrudeCommand(ModelObject modelObject, Vector3 point, IIdentityService identityService)
        {
            _modelObject = modelObject;
            _point = point;
            _identityService = identityService;
        }
        
        public void Execute()
        {
            _previousModel = _modelObject.Model;
            _extrusionResult = _modelObject.Extrude(_point);
            _identityService.Calculate(_modelObject.Model, _extrusionResult.ModifiedChunksId);
        }

        public void Undo()
        {
            _modelObject.ApplyModel(_previousModel, _extrusionResult.ModifiedChunksId);
            _identityService.Calculate(_modelObject.Model, _extrusionResult.ModifiedChunksId);
        }
    }
}
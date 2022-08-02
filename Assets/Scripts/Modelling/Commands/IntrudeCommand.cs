using Modelling.Services;
using UnityEngine;

namespace Modelling.Commands
{
    public class IntrudeCommand : ICommand
    {
        private readonly ModelObject _modelObject;
        private readonly Vector3 _point;
        
        private Model _previousModel;
        private IntrusionResult _intrusionResult;

        public IntrudeCommand(ModelObject modelObject, Vector3 point)
        {
            _modelObject = modelObject;
            _point = point;
        }
        
        public void Execute()
        {
            _previousModel = _modelObject.Model;
            _intrusionResult = _modelObject.Intrude(_point);
        }

        public void Undo()
        {
            _modelObject.ApplyModel(_previousModel, _intrusionResult.ModifiedChunksId);
        }
    }
}
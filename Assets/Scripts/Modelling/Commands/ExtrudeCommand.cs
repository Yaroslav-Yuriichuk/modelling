using Modelling.Services;
using UnityEngine;

namespace Modelling.Commands
{
    public class ExtrudeCommand : ICommand
    {
        private readonly ModelObject _modelObject;
        private readonly Vector3 _point;
        
        private Model _previousModel;
        private ExtrusionResult _extrusionResult;

        public ExtrudeCommand(ModelObject modelObject, Vector3 point)
        {
            _modelObject = modelObject;
            _point = point;
        }
        
        public void Execute()
        {
            _previousModel = _modelObject.Model;
            _extrusionResult = _modelObject.Extrude(_point);
        }

        public void Undo()
        {
            _modelObject.ApplyModel(_previousModel, _extrusionResult.ModifiedChunksId);
        }
    }
}
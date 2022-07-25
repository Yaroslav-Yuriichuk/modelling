using Modelling.Services;
using UnityEngine;

namespace Modelling.Commands
{
    public class ExtrudeCommand : ICommand
    {
        private ModelObject _modelObject;
        private Model _previousModel;
        private Vector3 _point;

        private IExIntrusionService _exIntrusionService;
        
        public ExtrudeCommand(ModelObject modelObject, IExIntrusionService exIntrusionService, Vector3 point)
        {
            _exIntrusionService = exIntrusionService;
            _modelObject = modelObject;
            _point = point;
        }
        
        public void Execute()
        {
            _previousModel = _modelObject.Model;
            _modelObject.ApplyModel(_exIntrusionService.Extrude(_modelObject.Model, _point));
        }

        public void Undo()
        {
            _modelObject.ApplyModel(_previousModel);
        }
    }
}
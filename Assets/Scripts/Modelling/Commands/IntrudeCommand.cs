using Modelling.Services;
using UnityEngine;

namespace Modelling.Commands
{
    public class IntrudeCommand : ICommand
    {
        private ModelObject _modelObject;
        private Model _previousModel;
        private Vector3 _point;
        
        private readonly IExIntrusionService _exIntrusionService;
        
        public IntrudeCommand(ModelObject modelObject, IExIntrusionService exIntrusionService, Vector3 point)
        {
            _exIntrusionService = exIntrusionService;
            _modelObject = modelObject;
            _point = point;
        }
        
        public void Execute()
        {
            _previousModel = _modelObject.Model;
            _modelObject.ApplyModel(_exIntrusionService.Intrude(_modelObject.Model, _point));
        }

        public void Undo()
        {
            _modelObject.ApplyModel(_previousModel);
        }
    }
}
using UnityEngine;

namespace Modelling.Services
{
    public interface IExIntrusionService
    {
        public float IntrusionStrength { get; set; }
        public float ExtrusionStrength { get; set; }
        
        public Model Intrude(Model model, Vector3 point);
        public Model Extrude(Model model, Vector3 point);
    }
}
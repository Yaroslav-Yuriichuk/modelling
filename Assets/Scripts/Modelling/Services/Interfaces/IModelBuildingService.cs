using UnityEngine;

namespace Modelling.Services
{
    public interface IModelBuildingService
    {
        public Mesh Build(Model model);
    }
}
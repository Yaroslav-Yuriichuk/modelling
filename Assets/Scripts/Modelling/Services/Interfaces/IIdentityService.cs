using System;

namespace Modelling.Services
{
    public interface IIdentityService
    {
        public event Action<float> OnIdentityCalculated;

        public void Calculate(Model firstModel, Model secondModel);
    }
}
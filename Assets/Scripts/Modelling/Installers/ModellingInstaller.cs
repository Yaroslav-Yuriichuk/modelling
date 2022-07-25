using Modelling.Services;
using UnityEngine;
using Zenject;
using ILogger = Modelling.Services.ILogger;
using Logger = Modelling.Services.Logger;

namespace Modelling
{
    public class ModellingInstaller : MonoInstaller
    {
        [SerializeField] private ModelObject _model;
        
        public override void InstallBindings()
        {
            BindModelGeneratingService();
            BindExIntrusionService();
            BindInputService();
            BindModellingService();
            BindModelBuildingService();
            BindIdentityService();
            BindModelObject();
            BindLogger();
        }

        private void BindIdentityService()
        {
            Container
                .Bind<IIdentityService>()
                .To<IdentityService>()
                .AsSingle()
                .NonLazy();
        }

        private void BindModelBuildingService()
        {
            Container
                .Bind<IModelBuildingService>()
                .To<ModelBuildingServiceCPUGPU>()
                .AsSingle()
                .NonLazy();
        }

        private void BindLogger()
        {
            Container
                .Bind<ILogger>()
                .To<Logger>()
                .AsSingle()
                .NonLazy();
        }

        private void BindModellingService()
        {
            Container
                .Bind<IModellingService>()
                .To<ModellingService>()
                .AsSingle()
                .NonLazy();
        }

        private void BindInputService()
        {
            Container
                .BindInterfacesTo<InputService>()
                .AsSingle()
                .NonLazy();
        }

        private void BindExIntrusionService()
        {
            Container
                .Bind<IExIntrusionService>()
                .To<ExIntrusionService>()
                .AsSingle()
                .NonLazy();
        }

        private void BindModelObject()
        {
            Container
                .Bind<ModelObject>()
                .FromComponentInNewPrefab(_model)
                .AsTransient();
        }

        private void BindModelGeneratingService()
        {
            Container
                .Bind<IModelGeneratingService>()
                .To<ModelGeneratingService>()
                .AsSingle()
                .NonLazy();
        }
    }
}
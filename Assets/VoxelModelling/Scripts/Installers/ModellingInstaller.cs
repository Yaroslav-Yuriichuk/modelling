using UnityEngine;
using Zenject;
using VoxelModelling.Services;
using ILogger = VoxelModelling.Services.ILogger;
using Logger = VoxelModelling.Services.Logger;

namespace VoxelModelling
{
    public class ModellingInstaller : MonoInstaller
    {
        [SerializeField] private ModelObject _model;
        
        public override void InstallBindings()
        {
            BindModelGeneratingService();
            BindExIntrusionService();
            BindBuildingService();
            BindModelObject();
            BindLogger();
        }

        private void BindBuildingService()
        {
            Container
                .Bind<IBuildingService>()
                .To<BuildingService>()
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
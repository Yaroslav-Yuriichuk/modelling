﻿using Modelling.Services;
using Modelling.UI;
using UnityEngine;
using Zenject;
using ILogger = Modelling.Services.ILogger;
using Logger = Modelling.Services.Logger;

namespace Modelling
{
    public class ModellingInstaller : MonoInstaller
    {
        [SerializeField] private ModelObject _model;
        [SerializeField] private Loading _loading;
        
        public override void InstallBindings()
        {
            BindModelGeneratingService();
            BindExIntrusionService();
            BindInputService();
            BindModellingService();
            BindBuildingService();
            BindIdentityService();
            BindModelObject();
            BindLoading();
            BindLoadingService();
            BindLogger();
        }

        private void BindLoading()
        {
            Container
                .Bind<Loading>()
                .FromComponentInNewPrefab(_loading)
                .AsSingle();
        }

        private void BindLoadingService()
        {
            Container
                .Bind<ILoadingService>()
                .To<LoadingService>()
                .AsSingle()
                .NonLazy();
        }

        private void BindBuildingService()
        {
            Container
                .Bind<IBuildingService>()
                .To<BuildingService>()
                .AsSingle()
                .NonLazy();
        }

        private void BindIdentityService()
        {
            Container
                .Bind<IIdentityService>()
                .To<IdentityService>()
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
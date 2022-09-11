using Modelling.Audio;
using Modelling.Services;
using Modelling.UI;
using UnityEngine;
using Zenject;

namespace Modelling
{
    public class ModellingAppInstaller : MonoInstaller
    {
        [SerializeField] private LoadingElement _loadingElement;
        [SerializeField] private AudioPlayer _audioPlayer;
        
        public override void InstallBindings()
        {
            BindInputService();
            BindModellingService();
            BindIdentityService();
            BindLoadingService();
            BindAudioService();
        }

        private void BindAudioService()
        {
            Container
                .Bind<AudioPlayer>()
                .FromComponentInNewPrefab(_audioPlayer)
                .AsSingle()
                .NonLazy();

            Container
                .Bind<IAudioService>()
                .To<AudioService>()
                .AsSingle()
                .NonLazy();
        }

        private void BindLoadingService()
        {
            Container
                .Bind<LoadingElement>()
                .FromComponentInNewPrefab(_loadingElement)
                .AsSingle();
            
            Container
                .Bind<ILoadingService>()
                .To<LoadingService>()
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
    }
}
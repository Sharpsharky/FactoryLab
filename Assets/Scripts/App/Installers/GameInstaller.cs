using System.ComponentModel;
using UnityEngine;
using Zenject;
using FactoryLab.Core.Data;
using FactoryLab.Core.Domain;
using FactoryLab.Core.Interfaces;
using FactoryLab.App.Factory;
using FactoryLab.App.Controllers;

namespace FactoryLab.App.Installers
{
    public class GameInstaller : MonoInstaller
    {
        [SerializeField] private ElementLibrarySO _library;
        [SerializeField] private Camera _mainCamera;

        public override void InstallBindings()
        {
            Container.BindInstance(_library).AsSingle();
            Container.BindInstance(_mainCamera).AsSingle();

            Container.Bind<LayoutState>().AsSingle();
            Container.Bind<ElementViewFactory>().AsSingle();

            Container.Bind<TableController>()
                .AsSingle()
                .NonLazy();

            Container.Bind<IElementSpawner>()
                .To<TableController>()
                .FromResolve();

            Container.BindInterfacesAndSelfTo<DragDropController>()
                .AsSingle()
                .NonLazy();

            Container.BindInterfacesAndSelfTo<PortConnectionController>()
                .AsSingle()
                .NonLazy();

#if UNITY_EDITOR
            Container.Bind<DebugSpawner>()
                .FromNewComponentOnNewGameObject()
                .AsSingle()
                .NonLazy();
#endif
        }
    }
}

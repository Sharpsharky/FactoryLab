using System.Collections.Generic;
using UnityEngine;
using Zenject;
using FactoryLab.Core;
using FactoryLab.Core.Data;
using FactoryLab.Core.Domain;
using FactoryLab.Core.Interfaces;
using FactoryLab.Core.Validation;
using FactoryLab.App.Factory;
using FactoryLab.App.Controllers;

namespace FactoryLab.App.Installers
{
    public class GameInstaller : MonoInstaller
    {
        [SerializeField] private ElementLibrarySO  _library;
        [SerializeField] private LayoutTemplateSO  _layoutTemplate;
        [SerializeField] private Camera            _mainCamera;
        [SerializeField] private EvaluationMode    _evaluationMode = EvaluationMode.Learning;

        public override void InstallBindings()
        {
            Container.BindInstance(_library).AsSingle();
            Container.BindInstance(_layoutTemplate).AsSingle();
            Container.BindInstance(_mainCamera).AsSingle();
            Container.BindInstance(_evaluationMode).AsSingle();

            Container.Bind<LayoutState>().AsSingle();
            Container.Bind<ElementViewFactory>().AsSingle();

            Container.Bind<TableController>().AsSingle().NonLazy();
            Container.Bind<IElementSpawner>().To<TableController>().FromResolve();

            BindValidators();

            Container.BindInterfacesAndSelfTo<DragDropController>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<PortConnectionController>().AsSingle().NonLazy();

#if UNITY_EDITOR
            Container.Bind<DebugSpawner>()
                .FromNewComponentOnNewGameObject()
                .AsSingle()
                .NonLazy();
#endif
        }

        private void BindValidators()
        {
            Container.Bind<ILayoutValidator>().To<ConnectionCompatibilityValidator>().AsSingle();
            Container.Bind<ILayoutValidator>().To<OutputCompletenessValidator>().AsSingle();
            Container.Bind<ILayoutValidator>().To<TemplateComplianceValidator>().AsSingle();

            Container.Bind<ValidationService>().AsSingle();
        }
    }
}

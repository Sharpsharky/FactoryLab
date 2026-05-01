using Zenject;
using FactoryLab.Ui.Views;

namespace FactoryLab.Ui.Installers
{
    public class UIInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<HUDView>().FromComponentInHierarchy().AsSingle();
            Container.Bind<LibraryPanelView>().FromComponentInHierarchy().AsSingle();
            Container.Bind<ValidationPanelView>().FromComponentInHierarchy().AsSingle();
            Container.Bind<ContextMenuView>().FromComponentInHierarchy().AsSingle();
        }
    }
}

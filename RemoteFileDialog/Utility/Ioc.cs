using System.Windows;
using RemoteMusicPlayerClient.CustomFrameworkElements.DryIoc;
using RemoteMusicPlayerClient.CustomFrameworkElements.Entries;
using RemoteMusicPlayerClient.CustomFrameworkElements.Utility.Validators;

namespace RemoteMusicPlayerClient.CustomFrameworkElements.Utility
{
    public static class Ioc
    {
        static Ioc()
        {
            RegisterAllDependencies();
        }
        public static IContainer Container = new Container();
        public static void RegisterAllDependencies()
        {
            Container.Register<IEntryViewModel, EntryViewModel>();

            Container.Register<IEntryService, EntryService>(Reuse.Singleton);

            Container.Register<EntryExistsValidationRule>(Reuse.Singleton);

            Container.Register<IRemoteFileDialogViewModel, RemoteFileDialogViewModel>(Reuse.Singleton);
        }

        public static void SetStaticResources(ResourceDictionary resources)
        {
            resources[nameof(EntryExistsValidationRule)] = Container.Resolve<EntryExistsValidationRule>();
        }
    }
}
using System.Net.Http;
using System.Windows;
using RemoteMusicPlayerClient.CustomFrameworkElements.RemoteFileDialog.DryIoc;
using RemoteMusicPlayerClient.CustomFrameworkElements.RemoteFileDialog.Entries;
using RemoteMusicPlayerClient.CustomFrameworkElements.RemoteFileDialog.Utility.Validators;

namespace RemoteMusicPlayerClient.CustomFrameworkElements.RemoteFileDialog.Utility
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
            Container.Register<IDialogModeService, DialogModeService>(Reuse.Singleton);
            Container.Register<IRemoteFileDialogViewModel, RemoteFileDialogViewModel>(Reuse.Singleton);

            Container.Register<IEntryViewModel, EntryViewModel>();
            Container.Register<IEntryService, EntryService>(Reuse.Singleton);

            Container.Register<EntryExistsValidationRule>(Reuse.Singleton);
            Container.Register<HttpClient>(Reuse.Singleton);
        }

        public static void SetStaticResources(ResourceDictionary resources)
        {
            resources[nameof(EntryExistsValidationRule)] = Container.Resolve<EntryExistsValidationRule>();
        }
    }
}
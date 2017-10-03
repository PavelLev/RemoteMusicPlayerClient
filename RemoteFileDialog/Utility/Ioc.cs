using DryIoc;
using RemoteFileDialog.Entries;
using RemoteFileDialog.Services;
using RemoteFileDialog.Utility.Validators;

namespace RemoteFileDialog.Utility
{
    public class Ioc
    {
        static Ioc()
        {
            RegisterAllDependencies();
        }
        
        public static IContainer Container { get; } = new Container();

        public static void RegisterAllDependencies()
        {
            Container.Register<IEntryViewModel, EntryViewModel>();

            Container.Register<IEntryService, EntryService>();

            Container.Register<EntryExistsValidationRule>();

            Container.Register<IRemoteFileDialogViewModel, RemoteFileDialogViewModel>();
        }
    }
}
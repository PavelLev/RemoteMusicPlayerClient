using System.Collections.Generic;
using System.Threading.Tasks;
using RemoteMusicPlayerClient.CustomFrameworkElements.RemoteFileDialog.Utility;
using RemoteMusicPlayerClient.CustomFrameworkElements.RemoteFileDialog.DryIoc;

namespace RemoteMusicPlayerClient.CustomFrameworkElements.RemoteFileDialog
{
    public partial class RemoteFileDialog
    {
        private static Task _uncheckAllTask;

        private RemoteFileDialog(DialogMode dialogMode)
        {
            Ioc.SetStaticResources(Resources);

            InitializeComponent();
            DataContext = Ioc.Container.Resolve<IRemoteFileDialogViewModel>();

            var dialogModeService = Ioc.Container.Resolve<IDialogModeService>();
            dialogModeService.Current = dialogMode;
        }

        private void Window_Closed(object sender, System.EventArgs e)
        {
            _uncheckAllTask = Task.Run(() => ViewModel.UncheckAll());
        }

        public static async Task<RemoteFileDialog> CreateAsync(DialogMode dialogMode)
        {
            if (_uncheckAllTask != null)
            {
                await _uncheckAllTask;
            }

            return new RemoteFileDialog(dialogMode);
        }

        public List<string> SelectedFiles => ViewModel.SelectedFiles;
        public List<string> SelectedDirectories => ViewModel.SelectedDirectories;

        public IRemoteFileDialogViewModel ViewModel => (IRemoteFileDialogViewModel)DataContext;
    }
}

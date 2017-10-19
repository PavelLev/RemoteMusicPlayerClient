using System.Collections.Generic;
using System.Threading.Tasks;
using RemoteMusicPlayerClient.CustomFrameworkElements.Utility;
using RemoteMusicPlayerClient.CustomFrameworkElements.DryIoc;

namespace RemoteMusicPlayerClient.CustomFrameworkElements
{
    public partial class RemoteFileDialog
    {
        private static Task _uncheckAllTask;

        public RemoteFileDialog()
        {
            Ioc.SetStaticResources(Resources);

            InitializeComponent();
            DataContext = Ioc.Container.Resolve<IRemoteFileDialogViewModel>();
        }

        public List<string> SelectedFiles => ViewModel.SelectedFiles;

        public IRemoteFileDialogViewModel ViewModel => (IRemoteFileDialogViewModel) DataContext;

        private void Window_Closed(object sender, System.EventArgs e)
        {
            _uncheckAllTask = Task.Run(() => ViewModel.UncheckAll());
        }

        public static async Task<RemoteFileDialog> CreateAsync()
        {
            if (_uncheckAllTask != null)
            {
                await _uncheckAllTask;
            }

            return new RemoteFileDialog();
        }
    }
}

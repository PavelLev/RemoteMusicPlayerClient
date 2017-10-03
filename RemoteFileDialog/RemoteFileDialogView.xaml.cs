using DryIoc;
using RemoteFileDialog.Utility;

namespace RemoteFileDialog
{
    public partial class RemoteFileDialogView
    {
        public RemoteFileDialogView()
        {
            InitializeComponent();
            DataContext = Ioc.Container.Resolve<IRemoteFileDialogViewModel>();
        }
    }
}

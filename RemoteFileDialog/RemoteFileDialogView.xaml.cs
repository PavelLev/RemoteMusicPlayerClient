using DryIoc;

namespace RemoteFileDialog
{
    public partial class RemoteFileDialogView
    {
        public RemoteFileDialogView(IContainer container)
        {
            InitializeComponent();
            DataContext = container.Resolve<IRemoteFileDialogViewModel>();
        }
    }
}

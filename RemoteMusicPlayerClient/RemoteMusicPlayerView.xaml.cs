using System.Windows;
using RemoteMusicPlayerClient.DryIoc;

namespace RemoteMusicPlayerClient
{
    public partial class RemoteMusicPlayerView
    {
        private readonly IResolver _resolver;

        public RemoteMusicPlayerView(IResolver resolver)
        {
            _resolver = resolver;
            InitializeComponent();
        }
    }
}

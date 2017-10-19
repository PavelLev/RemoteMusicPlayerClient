using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using RemoteMusicPlayerClient.Utility;
using RemoteMusicPlayerClient.DryIoc;

namespace RemoteMusicPlayerClient
{
    public partial class App
    {
        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            Ioc.SetStaticResources(Resources);

            Ioc.Container.Register<Junk>();
            Ioc.Container.Resolve<Junk>().DoShit();

            var remoteMusicPlayerView = Ioc.Container.Resolve<RemoteMusicPlayerView>();
            remoteMusicPlayerView.Show();
        }
    }
}

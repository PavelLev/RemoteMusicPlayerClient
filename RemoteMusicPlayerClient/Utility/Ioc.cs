using System.Windows;
using CSCore.SoundOut;
using Newtonsoft.Json;
using RemoteMusicPlayerClient.DryIoc;
using RemoteMusicPlayerClient.Music;
using RemoteMusicPlayerClient.Music.Playlisting;
using RemoteMusicPlayerClient.Networking;

namespace RemoteMusicPlayerClient.Utility
{
    public class Ioc
    {
        static Ioc()
        {
            RegisterAllDependencies();
        }
        public static IContainer Container = new Container();
        public static void RegisterAllDependencies()
        {
            Container.Register<IPlaylistCollectionViewModel, PlaylistCollectionViewModel>();
            Container.Register<IPlaylistFileDownloadStateService, PlaylistFileDownloadStateService>(Reuse.Singleton);
            Container.Register<PlaylistDirectoryViewModel>(setup: Setup.With(allowDisposableTransient: true));
            Container.Register<PlaylistFileViewModel>(setup: Setup.With(allowDisposableTransient: true));
            Container.Register<IPlaylistService, PlaylistService>(Reuse.Singleton);
            Container.Register<PlaylistViewModel>(setup: Setup.With(allowDisposableTransient: true));

            Container.Register<IMusicPlayerService, MusicPlayerService>();
            Container.Register<IPlayOrderService, PlayOrderService>();
            Container.UseInstance<ISoundOut>(new WasapiOut());

            Container.Register<IOnlineStatusService, OnlineStatusService>();
            Container.Register<RemoteFileReader>(setup: Setup.With(allowDisposableTransient: true));
            Container.Register<IRemoteFileReaderFactory, RemoteFileReaderFactory>(Reuse.Singleton);

            Container.Register<IApplicationNameService, ApplicationNameService>(Reuse.Singleton);
            Container.Register<IFileNameExtractor, FileNameExtractor>(Reuse.Singleton);
            Container.Register<IFileTypeHelper, FileTypeHelper>(Reuse.Singleton);
            Container.Register<JsonSerializer>();
            Container.Register<IToastService, ToastService>(Reuse.Singleton);

            Container.Register<IRemoteMusicPlayerViewModel, RemoteMusicPlayerViewModel>();
            Container.Register<RemoteMusicPlayerView>();
        }

        public static void SetStaticResources(ResourceDictionary resources)
        {
        }
    }
}
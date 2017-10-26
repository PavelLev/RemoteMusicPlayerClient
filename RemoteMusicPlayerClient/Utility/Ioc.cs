using System.Net.Http;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Windows;
using CSCore.SoundOut;
using Newtonsoft.Json;
using RemoteMusicPlayerClient.DryIoc;
using RemoteMusicPlayerClient.Music;
using RemoteMusicPlayerClient.Music.Playlisting;
using RemoteMusicPlayerClient.Networking;
using RemoteMusicPlayerClient.Networking.Files;

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
            Container.Register<IPlaylistCollectionViewModel, PlaylistCollectionViewModel>(Reuse.Singleton);
            Container.Register<IPlaylistFileDownloadStateService, PlaylistFileDownloadStateService>(Reuse.Singleton);
            Container.Register<PlaylistDirectoryViewModel>();
            Container.Register<PlaylistFileViewModel>();
            Container.Register<IPlaylistSaverService, PlaylistSaverService>(Reuse.Singleton);
            Container.Register<PlaylistViewModel>();

            Container.Register<IMusicPlayerService, MusicPlayerService>(Reuse.Singleton);
            Container.Register<IPlayOrderService, PlayOrderService>(Reuse.Singleton);
            Container.UseInstance<ISoundOut>(new WasapiOut());

            Container.Register<IFileService, FileService>(Reuse.Singleton);
            Container.Register<IOnlineStatusService, OnlineStatusService>(Reuse.Singleton);
            Container.Register<RemoteFileReader>(setup: Setup.With(allowDisposableTransient: true));
            Container.Register<IRemoteFileReaderFactory, RemoteFileReaderFactory>(Reuse.Singleton);

            Container.Register<IApplicationNameService, ApplicationNameService>(Reuse.Singleton);
            Container.Register<IFileNameExtractor, FileNameExtractor>(Reuse.Singleton);
            Container.Register<IFileTypeHelper, FileTypeHelper>(Reuse.Singleton);
            Container.Register<HttpClient>();
            Container.Register<JsonSerializer>();
            Container.Register<SHA256>();
            Container.Register<IToastService, ToastService>(Reuse.Singleton);

            Container.Register<IRemoteMusicPlayerViewModel, RemoteMusicPlayerViewModel>(Reuse.Singleton);
            Container.Register<RemoteMusicPlayerView>(Reuse.Singleton);
        }

        public static void SetStaticResources(ResourceDictionary resources)
        {
        }
    }
}
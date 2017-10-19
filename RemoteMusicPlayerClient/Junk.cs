using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using RemoteMusicPlayerClient.DryIoc;
using RemoteMusicPlayerClient.Music;
using RemoteMusicPlayerClient.Networking;
using RemoteMusicPlayerClient.Utility;

namespace RemoteMusicPlayerClient
{
    public class Junk
    {
        private readonly IMusicPlayerService _musicPlayerService;
        private readonly IContainer _container;
        private readonly IFileTypeHelper _fileTypeHelper;
        private static readonly string _filePath = "D:\\Offtop\\Music\\Mr. Robot OST\\Mac Quayle - Mr. Robot, Vol. 1 (2016)\\08. 1.0_8-whatsyourask.m4p.flac";

        public Junk(IMusicPlayerService musicPlayerService, IContainer container, IFileTypeHelper fileTypeHelper)
        {
            _musicPlayerService = musicPlayerService;
            _container = container;
            _fileTypeHelper = fileTypeHelper;
        }

        public async void DoShit()
        {
            var remoteFileReaderFactory = _container.Resolve<IRemoteFileReaderFactory>();

            var remoteFileReader = await remoteFileReaderFactory.ByPath(_filePath);

            _musicPlayerService.Initialize(_fileTypeHelper.GetFileType(_filePath), remoteFileReader);
            _musicPlayerService.Play();
            //_musicPlayerService.Stop();


            //_musicPlayerService.Position = (int)(_musicPlayerService.Length * 0.85);
            //_musicPlayerService.Play();
            //_musicPlayerService.Stop();

        }
    }
}
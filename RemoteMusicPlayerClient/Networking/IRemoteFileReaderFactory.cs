using System.Threading.Tasks;

namespace RemoteMusicPlayerClient.Networking
{
    public interface IRemoteFileReaderFactory
    {
        Task<RemoteFileReader> StartUse(string filePath);
        void StopUse(string filePath);
    }
}
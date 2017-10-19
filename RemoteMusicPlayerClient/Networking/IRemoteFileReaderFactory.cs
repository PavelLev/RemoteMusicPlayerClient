using System.Threading.Tasks;

namespace RemoteMusicPlayerClient.Networking
{
    public interface IRemoteFileReaderFactory
    {
        Task<RemoteFileReader> ByPath(string path);
        Task<string> GetTokenByPath(string path);
        Task<RemoteFileReader> ByToken(string token);
    }
}
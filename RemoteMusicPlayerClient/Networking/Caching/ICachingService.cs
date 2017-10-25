using System.Threading.Tasks;
using RemoteMusicPlayerClient.Utility.Segments;

namespace RemoteMusicPlayerClient.Networking.Caching
{
    public interface ICachingService
    {
        Task<int> InitializeAsync(string filePath, byte[] buffer);
        void Write(Segment segment);
        void Close();
    }
}
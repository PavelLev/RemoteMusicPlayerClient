using System.Threading.Tasks;

namespace RemoteMusicPlayerClient.Music
{
    public interface IMetadataService
    {
        Task<Metadata> GetAsync(string filePath);
    }
}
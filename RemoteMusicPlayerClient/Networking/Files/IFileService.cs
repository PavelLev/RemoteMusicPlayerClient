using System.Collections.Generic;
using System.Threading.Tasks;

namespace RemoteMusicPlayerClient.Networking.Files
{
    public interface IFileService
    {
        Task<List<string>> GetAllFilesAsync(string path);
    }
}
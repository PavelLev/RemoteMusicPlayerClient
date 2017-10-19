using System.Collections.Generic;
using System.Threading.Tasks;

namespace RemoteMusicPlayerClient.CustomFrameworkElements.Entries
{
    public interface IEntryService
    {
        Task<IEnumerable<Entry>> GetChildEntriesAsync(string path, bool recursive = false);
        Task<IEnumerable<Entry>> GetRootEntriesAsync();
        Task<bool> EntryExists(string path);
    }
}
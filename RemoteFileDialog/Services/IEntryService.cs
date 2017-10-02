using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using RemoteFileDialog.Entries;

namespace RemoteFileDialog.Services
{
    public interface IEntryService
    {
        Task<IEnumerable<Entry>> LoadChildEntriesAsync(string path, bool recursive = false);
        IEnumerable<Entry> LoadRootEntries();
        bool EntryExists(string path);
    }
}
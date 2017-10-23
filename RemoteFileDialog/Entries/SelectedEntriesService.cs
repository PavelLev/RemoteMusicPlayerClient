using System.Collections.Generic;
using System.Linq;

namespace RemoteMusicPlayerClient.CustomFrameworkElements.RemoteFileDialog.Entries
{
    public class SelectedEntriesService : ISelectedEntriesService
    {
        private ICollection<Entry> SelectedEntries { get; } = new List<Entry>();
        public void Add(Entry entry)
        {
            SelectedEntries.Add(entry);
        }

        public bool Remove(Entry entry)
        {
            return SelectedEntries.Remove(entry);
        }

        public List<string> GetFilePathList()
        {
            return SelectedEntries.Where(entry => !entry.IsDirectory).Select(entry => entry.Path).ToList();
        }

        public List<string> GetDirectoryPathList()
        {
            return SelectedEntries.Where(entry => entry.IsDirectory).Select(entry => entry.Path).ToList();
        }
    }
}
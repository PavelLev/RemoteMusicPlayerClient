using System.Collections.Generic;

namespace RemoteMusicPlayerClient.CustomFrameworkElements.Entries
{
    public class SelectedEntriesService : ISelectedEntriesService
    {
        public SelectedEntriesService()
        {
            
        }
        public ICollection<Entry> SelectedEntries { get; } = new List<Entry>();
    }
}
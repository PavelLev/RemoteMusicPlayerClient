using System.Collections.Generic;

namespace RemoteMusicPlayerClient.CustomFrameworkElements.Entries
{
    public interface ISelectedEntriesService
    {
        ICollection<Entry> SelectedEntries { get; }
    }
}
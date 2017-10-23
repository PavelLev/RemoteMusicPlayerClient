using System.Collections.Generic;

namespace RemoteMusicPlayerClient.CustomFrameworkElements.RemoteFileDialog.Entries
{
    public interface ISelectedEntriesService
    {
        void Add(Entry entry);
        bool Remove(Entry entry);
        List<string> GetFilePathList();
        List<string> GetDirectoryPathList();
    }
}
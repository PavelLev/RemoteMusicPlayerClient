using RemoteFileDialog.Entries;

namespace RemoteFileDialog.Services
{
    public interface IEntryService
    {
        void LoadChildEntries(IEntry entry);
        void LoadChildEntriesRecursively(IEntry entry);
    }
}
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace RemoteFileDialog.Entries
{
    public interface IEntry : INotifyPropertyChanged
    {
        string Path { get; set; }
        string Name { get; set; }
        bool IsDirectory { get; set; }
        IList<IEntry> ChildEntries { get; set; }
    }
}
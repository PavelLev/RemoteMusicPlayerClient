using System.Collections.ObjectModel;
using System.ComponentModel;

namespace RemoteFileDialog.Entries
{
    public interface IEntryViewModel : INotifyPropertyChanged
    {
        Entry Entry { get; set; }
        ObservableCollection<IEntryViewModel> ChildEntryViewModels { get; set; }
        bool IsExpanded { get; set; }
        bool IsChecked { get; set; }
    }
}
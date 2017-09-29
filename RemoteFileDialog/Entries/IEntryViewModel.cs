using System.Collections.ObjectModel;
using System.ComponentModel;

namespace RemoteFileDialog.Entries
{
    public interface IEntryViewModel : INotifyPropertyChanged
    {
        IEntry Entry { get; set; }
        ObservableCollection<IEntryViewModel> ChildEntryViewModels { get; set; }
        bool IsExpanded { get; set; }
        bool IsChecked { get; set; }
    }
}
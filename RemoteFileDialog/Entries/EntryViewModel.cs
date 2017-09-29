using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Prism.Mvvm;

namespace RemoteFileDialog.Entries
{
    public class EntryViewModel : BindableBase, IEntryViewModel
    {
        private ObservableCollection<IEntryViewModel> _childEntryViewModels;
        private IEntry _entry;

        private bool _isChecked;

        private bool _isExpanded;

        public EntryViewModel()
        {
        }

        private void ChildEntriesChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (propertyChangedEventArgs.PropertyName != nameof(_entry.ChildEntries))
            {
                return;
            }

            if (_entry.ChildEntries == null)
            {
                return;
            }

            _childEntryViewModels =
                new ObservableCollection<IEntryViewModel>(_entry.ChildEntries.Select(entry =>
                    new EntryViewModel {Entry = entry}));
        }

        public IEntry Entry
        {
            get => _entry;
            set
            {
                SetProperty(ref _entry, value);

                if (_entry != null)
                {
                    _entry.PropertyChanged += ChildEntriesChanged;
                }
            }
        }

        public ObservableCollection<IEntryViewModel> ChildEntryViewModels
        {
            get => _childEntryViewModels;
            set => SetProperty(ref _childEntryViewModels, value);
        }

        public bool IsExpanded
        {
            get => _isExpanded;
            set => SetProperty(ref _isExpanded, value);
        }

        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                SetProperty(ref _isChecked, value);


            }
        }
    }
}
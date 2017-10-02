using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DryIoc;
using Prism.Mvvm;
using RemoteFileDialog.Services;

namespace RemoteFileDialog.Entries
{
    public class EntryViewModel : BindableBase, IEntryViewModel
    {
        private ObservableCollection<IEntryViewModel> _childEntryViewModels;
        private Entry _entry;

        private bool _isChecked;

        private bool _isExpanded;

        private ICollection<Entry> _selectedEntries;
        private readonly IContainer _container;
        private readonly IEntryService _entryService;

        public EntryViewModel(ICollection<Entry> selectedEntries, IContainer container, IEntryService entryService)
        {
            _selectedEntries = selectedEntries;
            _container = container;
            _entryService = entryService;
        }

        private void ChildEntriesChanged(object sender, System.ComponentModel.PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (propertyChangedEventArgs.PropertyName != nameof(_entry.ChildEntries))
            {
                return;
            }

            if (_entry.ChildEntries == null)
            {
                return;
            }

            ChildEntryViewModels =
                new ObservableCollection<IEntryViewModel>(_entry.ChildEntries.Select(childEntry =>
                {
                    var entryViewModel = _container.Resolve<IEntryViewModel>();
                    entryViewModel.Entry = childEntry;
                    return entryViewModel;
                }));
        }

        public Entry Entry
        {
            get => _entry;
            set
            {
                SetProperty(ref _entry, value);

                if (_entry != null)
                {
                    _entry.PropertyChanged += ChildEntriesChanged;

                    if (_entry.IsDirectory)
                    {
                        var dummyEntryViewModel = _container.Resolve<IEntryViewModel>();
                        dummyEntryViewModel.Entry = new Entry
                        {
                            Name = "Dummy"
                        };

                        ChildEntryViewModels = new ObservableCollection<IEntryViewModel>(new List<IEntryViewModel>
                        {
                           dummyEntryViewModel
                        });
                    }
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
            set
            {
                SetProperty(ref _isExpanded, value);

                if (value)
                {
                    if (_entry.ChildEntries == null)
                    {
                        _entry.ChildEntries = _entryService.LoadChildEntriesAsync(_entry.Path).Result.ToList();
                    }
                }
            }
        }

        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                SetProperty(ref _isChecked, value);

                if (value)
                {
                    if (_entry.ChildEntries == null)
                    {
                        _entry.ChildEntries = _entryService.LoadChildEntriesAsync(_entry.Path, true).Result.ToList();
                    }
                }

                foreach (var childEntryViewModel in _childEntryViewModels)
                {
                    childEntryViewModel.IsChecked = true;
                }
            }
        }
    }
}
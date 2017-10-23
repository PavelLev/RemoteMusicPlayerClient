using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Prism.Mvvm;
using RemoteMusicPlayerClient.CustomFrameworkElements.RemoteFileDialog.DryIoc;
using IContainer = RemoteMusicPlayerClient.CustomFrameworkElements.RemoteFileDialog.DryIoc.IContainer;

namespace RemoteMusicPlayerClient.CustomFrameworkElements.RemoteFileDialog.Entries
{
    public class EntryViewModel : BindableBase, IEntryViewModel
    {
        private readonly IContainer _container;
        private readonly IEntryService _entryService;

        private readonly SelectedEntriesService _selectedEntriesService;
        private ObservableCollection<IEntryViewModel> _childEntryViewModels;
        private Entry _entry;

        private bool _isChecked;

        private bool _isExpanded;

        public EntryViewModel(SelectedEntriesService selectedEntriesService, IContainer container,
            IEntryService entryService)
        {
            _selectedEntriesService = selectedEntriesService;
            _container = container;
            _entryService = entryService;
        }

        public Entry Entry
        {
            get => _entry;
            set
            {
                SetProperty(ref _entry, value);

                if (_entry == null)
                {
                    return;
                }


                _entry.PropertyChanged += ChildEntriesChanged;

                if (!_entry.IsDirectory)
                {
                    return;
                }

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

                if (value && _entry.ChildEntries == null)
                {
                    _entry.ChildEntries = _entryService.GetChildEntriesAsync(_entry.Path).Result.ToList();
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
                    _selectedEntriesService.Add(Entry);

                    if (_entry.ChildEntries == null)
                    {
                        _entry.ChildEntries = _entryService.GetChildEntriesAsync(_entry.Path, true).Result.ToList();
                    }
                }
                else
                {
                    _selectedEntriesService.Remove(Entry);
                }


                foreach (var childEntryViewModel in _childEntryViewModels)
                {
                    childEntryViewModel.IsChecked = value;
                }
            }
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

            ChildEntryViewModels =
                new ObservableCollection<IEntryViewModel>(_entry.ChildEntries.Select(childEntry =>
                {
                    var entryViewModel = _container.Resolve<IEntryViewModel>();
                    entryViewModel.Entry = childEntry;
                    return entryViewModel;
                }));
        }
    }
}
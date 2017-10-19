using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Prism.Commands;
using Prism.Mvvm;
using RemoteMusicPlayerClient.CustomFrameworkElements.DryIoc;
using RemoteMusicPlayerClient.CustomFrameworkElements.Entries;
using RemoteMusicPlayerClient.CustomFrameworkElements.Utility.Validators;

namespace RemoteMusicPlayerClient.CustomFrameworkElements
{
    public class RemoteFileDialogViewModel : BindableBase, IRemoteFileDialogViewModel
    {
        private readonly IEntryService _entryService;
        private readonly IResolver _resolver;
        private readonly ISelectedEntriesService _selectedEntriesService;
        private ICollection<IEntryViewModel> _rootEntryViewModels;
        private string _entryToCheckPath;

        public RemoteFileDialogViewModel(IEntryService entryService, IResolver resolver, ISelectedEntriesService selectedEntriesService)
        {
            _entryService = entryService;
            _resolver = resolver;
            _selectedEntriesService = selectedEntriesService;

            OkCommand = new DelegateCommand<Window>(OkCommandAction);
            CancelCommand = new DelegateCommand<Window>(CancelCommandAction);

            LoadRootEntries();

            EntryExistsValidationRule = resolver.Resolve<EntryExistsValidationRule>();
        }

        private async void LoadRootEntries()
        {
            RootEntryViewModels = (await _entryService.GetRootEntriesAsync()).Select(entry =>
            {
                var entryViewModel = _resolver.Resolve<IEntryViewModel>();
                entryViewModel.Entry = entry;
                return entryViewModel;
            }).ToList();
        }

        public void CheckEntry(string path)
        {
            var parts = path.Split('\\');
            parts[0] = parts[0].Substring(0, parts[0].Length - 1);

            var entryViewModels = RootEntryViewModels;

            foreach (var part in parts)
            {
                var entryViewModelToSelect = entryViewModels.FirstOrDefault(entryViewModel => entryViewModel.Entry.Name == part);
                if (entryViewModelToSelect == null)
                {
                    return;
                }
                entryViewModelToSelect.IsExpanded = true;
                entryViewModels = entryViewModelToSelect.ChildEntryViewModels;
            }
        }

        public void UncheckAll()
        {
            foreach (var rootEntryViewModel in RootEntryViewModels)
            {
                rootEntryViewModel.IsChecked = false;
            }
        }

        public void OkCommandAction(Window window)
        {
            window.DialogResult = true;
            SelectedFiles = _selectedEntriesService.SelectedEntries.Where(entry => !entry.IsDirectory).Select(entry => entry.Path).ToList();
            window.Close();
        }

        public void CancelCommandAction(Window window)
        {
            window.DialogResult = false;
            window.Close();
        }


        public ICollection<IEntryViewModel> RootEntryViewModels
        {
            get => _rootEntryViewModels;
            set => SetProperty(ref _rootEntryViewModels, value);
        }

        public DelegateCommand<Window> OkCommand { get; }

        public DelegateCommand<Window> CancelCommand { get; }

        public string EntryToCheckPath
        {
            get => _entryToCheckPath;
            set
            {
                SetProperty(ref _entryToCheckPath, value);
                CheckEntry(value);
            }
        }

        public List<string> SelectedFiles { get; private set; }

        public EntryExistsValidationRule EntryExistsValidationRule { get; private set; }
    }
}
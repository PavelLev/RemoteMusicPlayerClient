using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using DryIoc;
using Prism.Commands;
using Prism.Mvvm;
using RemoteFileDialog.Entries;
using RemoteFileDialog.Services;
using RemoteFileDialog.Utility.Validators;

namespace RemoteFileDialog
{
    public class RemoteFileDialogViewModel : BindableBase, IRemoteFileDialogViewModel
    {
        private ICollection<IEntryViewModel> _rootEntryViewModels;
        private DelegateCommand<Window> _ok;
        private DelegateCommand<Window> _cancelCommand;
        private string _entryToCheckPath;

        public RemoteFileDialogViewModel(IEntryService entryService, IContainer container)
        {
            RootEntryViewModels = entryService.LoadRootEntries().Select(entry =>
            {
                var entryViewModel = container.Resolve<IEntryViewModel>();
                entryViewModel.Entry = entry;
                return entryViewModel;
            }).ToList();
            EntryExistsValidationRule = container.Resolve<EntryExistsValidationRule>();
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


        public ICollection<IEntryViewModel> RootEntryViewModels
        {
            get => _rootEntryViewModels;
            set => SetProperty(ref _rootEntryViewModels, value);
        }

        public DelegateCommand<Window> OkCommand => _ok ?? (_ok = new DelegateCommand<Window>(
            window =>
            {
                window.DialogResult = true;
            }));

        public DelegateCommand<Window> CancelCommand => _cancelCommand ?? (_cancelCommand = new DelegateCommand<Window>(
            window =>
            {
                window.DialogResult = false;
            }));

        public string EntryToCheckPath
        {
            get => _entryToCheckPath;
            set
            {
                SetProperty(ref _entryToCheckPath, value);
                CheckEntry(value);
            }
        }

        public EntryExistsValidationRule EntryExistsValidationRule { get; private set; }
    }
}
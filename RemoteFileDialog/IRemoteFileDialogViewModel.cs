using System.Collections.Generic;
using System.Windows;
using Prism.Commands;
using RemoteFileDialog.Entries;

namespace RemoteFileDialog
{
    public interface IRemoteFileDialogViewModel
    {
        void CheckEntry(string path);
        ICollection<IEntryViewModel> RootEntryViewModels { get; set; }
        DelegateCommand<Window> OkCommand { get; }
        DelegateCommand<Window> CancelCommand { get; }
        string EntryToCheckPath { get; }
    }
}
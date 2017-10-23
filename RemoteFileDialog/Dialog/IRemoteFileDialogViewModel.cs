using System.Collections.Generic;
using System.Windows;
using Prism.Commands;
using RemoteMusicPlayerClient.CustomFrameworkElements.RemoteFileDialog.Entries;

namespace RemoteMusicPlayerClient.CustomFrameworkElements.RemoteFileDialog
{
    public interface IRemoteFileDialogViewModel
    {
        void CheckEntry(string path);
        void UncheckAll();
        ICollection<IEntryViewModel> RootEntryViewModels { get; set; }
        DelegateCommand<Window> OkCommand { get; }
        DelegateCommand<Window> CancelCommand { get; }
        string EntryToCheckPath { get; }
        List<string> SelectedFiles { get; }
        List<string> SelectedDirectories { get; }
    }
}
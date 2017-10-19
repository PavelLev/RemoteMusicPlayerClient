using System.Collections.Generic;
using System.Windows;
using Prism.Commands;
using RemoteMusicPlayerClient.CustomFrameworkElements.Entries;

namespace RemoteMusicPlayerClient.CustomFrameworkElements
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
    }
}
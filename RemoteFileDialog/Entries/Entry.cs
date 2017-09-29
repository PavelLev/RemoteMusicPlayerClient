using System.Collections.Generic;
using Prism.Mvvm;

namespace RemoteFileDialog.Entries
{
    public class Entry : BindableBase, IEntry
    {
        private string _path;

        public string Path
        {
            get => _path;
            set => SetProperty(ref _path, value);
        }

        private string _name;

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        private bool _isDirectory;

        public bool IsDirectory
        {
            get => _isDirectory;
            set => SetProperty(ref _isDirectory, value);
        }

        private IList<IEntry> _childEntries;

        public IList<IEntry> ChildEntries
        {
            get => _childEntries;
            set => SetProperty(ref _childEntries, value);
        }
    }
}
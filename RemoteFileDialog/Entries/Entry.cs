using System.Collections.Generic;
using Prism.Mvvm;

namespace RemoteFileDialog.Entries
{
    public class Entry : BindableBase
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

        private IList<Entry> _childEntries;

        public IList<Entry> ChildEntries
        {
            get => _childEntries;
            set => SetProperty(ref _childEntries, value);
        }
    }
}
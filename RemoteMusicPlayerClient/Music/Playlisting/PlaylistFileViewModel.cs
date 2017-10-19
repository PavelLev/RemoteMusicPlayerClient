using System;
using System.IO;
using Prism.Commands;
using Prism.Mvvm;
using RemoteMusicPlayerClient.Networking;
using RemoteMusicPlayerClient.Utility;

namespace RemoteMusicPlayerClient.Music.Playlisting
{
    public class PlaylistFileViewModel : BindableBase, IDisposable
    {
        private readonly IRemoteFileReaderFactory _remoteFileReaderFactory;
        private bool _isChecked;
        private Metadata _metadata;

        public Action<PlaylistFileViewModel> SelectedToPlay;
        private bool _isSelected;
        private RemoteFileReader _stream;

        public PlaylistFileViewModel(IFileNameExtractor fileNameExtractor, IFileTypeHelper fileTypeHelper, IRemoteFileReaderFactory remoteFileReaderFactory, string path, bool isChecked = true)
        {
            _remoteFileReaderFactory = remoteFileReaderFactory;
            Path = path;
            Name = fileNameExtractor.GetName(Path);
            Type = fileTypeHelper.GetFileType(Path);
            IsChecked = isChecked;

            SelectToPlayCommand = new DelegateCommand(SelectToPlay);
        }

        public void SelectToPlay()
        {
            SelectedToPlay?.Invoke(this);
        }

        public DelegateCommand SelectToPlayCommand { get; }

        public string Path { get; }
        public string Name { get; }
        public FileType Type { get; }

        public bool IsChecked
        {
            get => _isChecked;
            set => SetProperty(ref _isChecked, value);
        }

        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }

        public Metadata Metadata
        {
            get => _metadata;
            set => SetProperty(ref _metadata, value);
        }

        public RemoteFileReader Stream => _stream ?? (_stream = _remoteFileReaderFactory.ByPath(Path).Result);

        public void Dispose()
        {
            _stream?.Dispose();
        }
    }
}
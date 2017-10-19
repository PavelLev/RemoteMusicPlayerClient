using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Prism.Mvvm;

namespace RemoteMusicPlayerClient.Music.Playlisting
{
    public class PlaylistViewModel : BindableBase, IDisposable
    {
        private ObservableCollection<PlaylistDirectoryViewModel> _directories;
        private string _name;
        
        public void Remove(PlaylistFileViewModel playlistFileViewModel)
        {
            var playlistFileFound = Directories.Any(playlistDirectoryViewModel =>
                playlistDirectoryViewModel.Files.Remove(playlistFileViewModel));

            playlistFileViewModel.Dispose();

            if (!playlistFileFound)
            {
                throw new ArgumentException("playlist doesn't contain specified playlistFile",
                    nameof(playlistFileViewModel));
            }
        }

        public void UpdateSelection(PlaylistDirectoryViewModel playlistDirectoryViewModel = null,
            PlaylistFileViewModel playlistFileViewModel = null, bool ShouldAppend = false, bool IsSegment = false)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            foreach (var playlistDirectoryViewModel in Directories)
            {
                playlistDirectoryViewModel.Dispose();
            }
        }

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public ObservableCollection<PlaylistDirectoryViewModel> Directories
        {
            get => _directories;
            set => SetProperty(ref _directories, value);
        }

        public PlaylistFileViewModel First => Directories.First().Files.First();
        public PlaylistFileViewModel Last => Directories.Last().Files.Last();
        public IEnumerable<PlaylistFileViewModel> AllFiles => Directories.SelectMany(directory => directory.Files);
    }
}
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Prism.Mvvm;

namespace RemoteMusicPlayerClient.Music.Playlisting
{
    public class PlaylistViewModel : BindableBase
    {
        private readonly IPlaylistService _playlistService;
        private ObservableCollection<PlaylistDirectoryViewModel> _directories;
        private string _name;

        public PlaylistViewModel(IPlaylistService playlistService)
        {
            _playlistService = playlistService;
        }

        public void AddSources(IEnumerable<string> newSourceDirectories)
        {
            _playlistService.AddSources(SourceDirectories, newSourceDirectories);
        }

        public void Rescan()
        {
            _playlistService.Rescan(this);
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

        public ObservableCollection<string> SourceDirectories { get; } = new ObservableCollection<string>();
    }
}
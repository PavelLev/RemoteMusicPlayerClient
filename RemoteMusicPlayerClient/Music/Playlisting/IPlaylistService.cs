using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace RemoteMusicPlayerClient.Music.Playlisting
{
    public interface IPlaylistService
    {
        void AddSources(ObservableCollection<string> sourceDirectories, IEnumerable<string> newSourceDirectories);
        void Rescan(PlaylistViewModel playlistViewModel);
        void LoadMetadata(IEnumerable<PlaylistFileViewModel> files);
    }
}
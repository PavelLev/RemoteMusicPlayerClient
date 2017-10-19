using System.Collections.Generic;

namespace RemoteMusicPlayerClient.Music.Playlisting
{
    public interface IPlaylistFileDownloadStateService
    {
        bool AddToDownloaded(PlaylistFileViewModel playlistFileViewModel);
        bool ContainsDownloaded(IEnumerable<PlaylistFileViewModel> playlistFileViewModels);
        bool ContainsDownloaded(PlaylistFileViewModel playlistFileViewModel);
        bool RemoveFromDownloaded(PlaylistFileViewModel playlistFileViewModel);
    }
}
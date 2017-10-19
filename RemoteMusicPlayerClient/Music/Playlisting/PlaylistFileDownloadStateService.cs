using System.Collections.Generic;
using System.Linq;

namespace RemoteMusicPlayerClient.Music.Playlisting
{
    public class PlaylistFileDownloadStateService: IPlaylistFileDownloadStateService
    {
        private readonly HashSet<PlaylistFileViewModel> _downloadedPlaylistFiles = new HashSet<PlaylistFileViewModel>(); 

        public bool AddToDownloaded(PlaylistFileViewModel playlistFileViewModel)
        {
            return _downloadedPlaylistFiles.Add(playlistFileViewModel);
        }

        public bool ContainsDownloaded(IEnumerable<PlaylistFileViewModel> playlistFileViewModels)
        {
            return playlistFileViewModels.Any(_downloadedPlaylistFiles.Contains);
        }

        public bool ContainsDownloaded(PlaylistFileViewModel playlistFileViewModel)
        {
            return _downloadedPlaylistFiles.Contains(playlistFileViewModel);
        }

        public bool RemoveFromDownloaded(PlaylistFileViewModel playlistFileViewModel)
        {
            return _downloadedPlaylistFiles.Remove(playlistFileViewModel);
        }
    }
}
using System.Collections.Generic;
using System.Windows.Documents;
using RemoteMusicPlayerClient.Music.Playlisting;

namespace RemoteMusicPlayerClient.Music
{
    public interface IPlayOrderService
    {
        List<PlaylistFileViewModel> Create(PlaylistFileViewModel firstFile, IEnumerable<PlaylistDirectoryViewModel> directories, bool shuffle);
    }
}
using System.Collections.Generic;

namespace RemoteMusicPlayerClient.Music.Playlisting.Collection
{
    public interface IPlaylistSaverService
    {
        void SaveAll();
        T LoadAll<T>() where T : IEnumerable<PlaylistViewModel>;
    }
}
using System.Collections.Generic;

namespace RemoteMusicPlayerClient.Music.Playlisting
{
    public interface IPlaylistSaverService
    {
        void SaveAll();
        T LoadAll<T>() where T : IEnumerable<PlaylistViewModel>;
    }
}
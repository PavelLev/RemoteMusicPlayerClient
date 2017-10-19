using System.Collections.Generic;

namespace RemoteMusicPlayerClient.Music.Playlisting
{
    public interface IPlaylistService
    {
        void Add(PlaylistViewModel playlist, List<string> files);
    }
}
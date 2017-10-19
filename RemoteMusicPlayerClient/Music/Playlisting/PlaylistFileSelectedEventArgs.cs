using System;

namespace RemoteMusicPlayerClient.Music.Playlisting
{
    public class PlaylistFileSelectedEventArgs : EventArgs
    {
        public bool ShouldAppend { get; set; }
        public PlaylistDirectoryViewModel PlaylistDirectoryViewModel { get; set; }
        public PlaylistFileViewModel PlaylistFileViewModel { get; set; }
    }
}
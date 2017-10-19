using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace RemoteMusicPlayerClient.Music.Playlisting
{
    public interface IPlaylistCollectionViewModel
    {
        event Action CurrentPlaylistChanged;
        ObservableCollection<PlaylistViewModel> All { get; }
        PlaylistViewModel Current { get; set; }
        PlaylistViewModel Next { get; }
        PlaylistViewModel Previous { get; }
        PlaylistViewModel Selected { get; set; }
        IEnumerable<PlaylistFileViewModel> AllFiles { get; }
    }
}
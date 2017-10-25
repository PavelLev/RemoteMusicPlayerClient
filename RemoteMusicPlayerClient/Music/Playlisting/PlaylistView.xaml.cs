using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using RemoteMusicPlayerClient.DryIoc;
using RemoteMusicPlayerClient.Utility;

namespace RemoteMusicPlayerClient.Music.Playlisting
{
    public partial class PlaylistView
    {
        public PlaylistView()
        {
            InitializeComponent();
        }

        public PlaylistViewModel PlaylistViewModel => (PlaylistViewModel) DataContext;
    }
}

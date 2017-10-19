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
            DataContext = new PlaylistViewModel
            {
                Name = "Playlist 1",
                Directories = new ObservableCollection<PlaylistDirectoryViewModel>
                {
                    new PlaylistDirectoryViewModel
                    {
                        Name = "2009 - Imagine Dragons - EP",
                        Files = new ObservableCollection<PlaylistFileViewModel>
                        {
                            Ioc.Container.Resolve<Func<string, PlaylistFileViewModel>>()(
                                "D:\\Offtop\\Music\\Imagine Dragons FLAC\\2009 - Imagine Dragons - EP\\01 I Need a Minute.flac"),
                            Ioc.Container.Resolve<Func<string, PlaylistFileViewModel>>()(
                                "D:\\Offtop\\Music\\Imagine Dragons FLAC\\2009 - Imagine Dragons - EP\\02 Uptight.flac"),
                            Ioc.Container.Resolve<Func<string, PlaylistFileViewModel>>()(
                                "D:\\Offtop\\Music\\Imagine Dragons FLAC\\2009 - Imagine Dragons - EP\\03 Cover Up.flac")
                        }
                    }
                }
            };
        }

        private void PlaylistFileMouseUp(object sender, MouseButtonEventArgs e)
        {
            var filesItemsControl = ((DependencyObject) sender).FindAncestor<ItemsControl>();
            var playlistDirectoryViewModel = (PlaylistDirectoryViewModel) filesItemsControl.DataContext;

            var playlistFileViewModel = (PlaylistFileViewModel) ((FrameworkElement) sender).DataContext;

            var shouldAppend = Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);

            var isSegment = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);

            PlaylistViewModel.UpdateSelection(playlistDirectoryViewModel, playlistFileViewModel, shouldAppend, isSegment);
        }

        public PlaylistViewModel PlaylistViewModel => (PlaylistViewModel) DataContext;
    }
}

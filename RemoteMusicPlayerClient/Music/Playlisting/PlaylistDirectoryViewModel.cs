using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Prism.Mvvm;

namespace RemoteMusicPlayerClient.Music.Playlisting
{
    public class PlaylistDirectoryViewModel : BindableBase, IDisposable
    {
        private ObservableCollection<PlaylistFileViewModel> _files;

        private void PartsAdded(IEnumerable<PlaylistFileViewModel> files)
        {
            foreach (var newFile in files)
            {
                newFile.PropertyChanged += (sender, notifyCollectionChangedEventArgs) =>
                {
                    var file = (PlaylistFileViewModel)sender;
                    if (notifyCollectionChangedEventArgs.PropertyName == nameof(file.IsChecked))
                    {
                        RaisePropertyChanged(nameof(IsChecked));
                    }
                };
            }
        }

        public void Dispose()
        {
            foreach (var playlistFileViewModel in Files)
            {
                playlistFileViewModel?.Dispose();
            }
        }

        public string Name { get; set; }

        public ObservableCollection<PlaylistFileViewModel> Files
        {
            get => _files;
            set
            {
                SetProperty(ref _files, value);
                if (value == null)
                {
                    return;
                }
                PartsAdded(value);
                value.CollectionChanged += (sender, notifyCollectionChangedEventArgs) =>
                {
                    if (notifyCollectionChangedEventArgs.NewItems.Count > 0)
                    {
                        PartsAdded(notifyCollectionChangedEventArgs.NewItems.Cast<PlaylistFileViewModel>());
                    }
                };
            }
        }

        public bool IsChecked
        {
            get => Files.All(playlistFile => playlistFile.IsChecked);
            set
            {
                foreach (var playlistFileViewModel in Files)
                {
                    playlistFileViewModel.IsChecked = value;
                }
                RaisePropertyChanged(nameof(IsChecked));
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Prism.Mvvm;
using RemoteMusicPlayerClient.DryIoc;

namespace RemoteMusicPlayerClient.Music.Playlisting
{
    public class PlaylistCollectionViewModel : BindableBase, IPlaylistCollectionViewModel
    {
        private readonly IContainer _container;
        private int _currentIndex;
        private PlaylistViewModel _selected;

        public PlaylistCollectionViewModel(IContainer container, ObservableCollection<PlaylistViewModel> playlists = null)
        {
            _container = container;

            All = playlists ?? new ObservableCollection<PlaylistViewModel>();
            PreventEmptiness();
            All.CollectionChanged += (sender, args) => PreventEmptiness();
        }

        public void PreventEmptiness()
        {
            if (All.Count != 0)
            {
                return;
            }
            var defaultPlaylist = _container.Resolve<PlaylistViewModel>();
            defaultPlaylist.Name = "Default";
            All.Add(defaultPlaylist);
        }

        public event Action CurrentPlaylistChanged;
        public ObservableCollection<PlaylistViewModel> All { get; }
        
        private int CurrentIndex
        {
            get => _currentIndex;
            set
            {
                if (Equals(_currentIndex, value))
                {
                    return;
                }
                _currentIndex = value;
                RaisePropertyChanged(nameof(Current));
                RaisePropertyChanged(nameof(Next));
                RaisePropertyChanged(nameof(Previous));
            }
        }

        public PlaylistViewModel Current
        {
            get => All[CurrentIndex];
            set => CurrentIndex = All.IndexOf(value);
        }

        /// <summary>
        /// Next playlist relative to Current
        /// </summary>
        public PlaylistViewModel Next =>
            CurrentIndex == All.Count - 1 ? All[0] : All[CurrentIndex + 1];

        /// <summary>
        /// Previous playlist relative to Current
        /// </summary>
        public PlaylistViewModel Previous =>
            CurrentIndex == 0 ? All.Last() : All[CurrentIndex - 1];

        public PlaylistViewModel Selected
        {
            get => _selected;
            set => SetProperty(ref _selected, value);
        }

        public IEnumerable<PlaylistFileViewModel> AllFiles => All.SelectMany(playlist => playlist.AllFiles);
    }

}
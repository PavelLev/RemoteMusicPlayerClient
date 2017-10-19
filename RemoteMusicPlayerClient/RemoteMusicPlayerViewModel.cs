using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using Prism.Commands;
using Prism.Mvvm;
using RemoteMusicPlayerClient.CustomFrameworkElements;
using RemoteMusicPlayerClient.Music;
using RemoteMusicPlayerClient.Music.Playlisting;
using RemoteMusicPlayerClient.Networking;

namespace RemoteMusicPlayerClient
{
    public class RemoteMusicPlayerViewModel : BindableBase, IRemoteMusicPlayerViewModel
    {
        private readonly IMusicPlayerService _musicPlayerService;
        private readonly IPlaylistService _playlistService;
        private readonly IPlayOrderService _playOrderService;
        private readonly IOnlineStatusService _onlineStatusService;
        private readonly IPlaylistFileDownloadStateService _playlistFileDownloadStateService;

        private int _playOrderCurrentFileIndex;
        private bool _repeatPlaylist;
        private bool _repeatTrack;
        private PlaylistFileViewModel _selectedFile;
        private bool _shuffle;
        private bool _stopOnPlaylistEnd;
        private OnlineStatus _currentOnlineStatus;

        public RemoteMusicPlayerViewModel(IMusicPlayerService musicPlayerService, IPlaylistService playlistService,
            IPlaylistCollectionViewModel playlistCollection, IPlayOrderService playOrderService, IOnlineStatusService onlineStatusService,
            IPlaylistFileDownloadStateService playlistFileDownloadStateService)
        {
            AddFilesCommand = new DelegateCommand(AddFilesCommandAction);

            _playlistService = playlistService;
            _playOrderService = playOrderService;
            _onlineStatusService = onlineStatusService;
            _playlistFileDownloadStateService = playlistFileDownloadStateService;
            _musicPlayerService = musicPlayerService;
            PlaylistCollection = playlistCollection;

            musicPlayerService.Stopped += MusicPlayerStopped;

            onlineStatusService.OnlineStatusChanged += OnlineStatusChanged;
        }

        private void OnlineStatusChanged(OnlineStatus newOnlineStatus)
        {
            CurrentOnlineStatus = newOnlineStatus;
        }

        public void MusicPlayerStopped()
        {

            if (RepeatTrack)
            {
                Play();
            }
            else if (!StopOnPlaylistEnd || PlayOrderCurrentFile != PlayOrderFiles.Last())
            {
                PlayNext();
            }
        }

        public void PlayNext()
        {
            var nextFile = PlayOrderNextFile;

            var playOrderIsFinished = nextFile == PlaylistCollection.Next.First;

            if (playOrderIsFinished || Shuffle)
            {
                if (playOrderIsFinished)
                {
                    PlaylistCollection.Current = PlaylistCollection.Next;
                }

                PlayOrderFiles = _playOrderService.Create(nextFile, PlaylistCollection.Current.Directories, Shuffle);
            }

            PlayOrderCurrentFile = nextFile;
        }

        public void PlayPrevious()
        {
            var previousFile = PlayOrderPreviousFile;

            var playOrderIsFinished = previousFile == PlaylistCollection.Previous.Last;

            if (playOrderIsFinished || Shuffle)
            {
                if (playOrderIsFinished)
                {
                    PlaylistCollection.Current = PlaylistCollection.Previous;
                }

                PlayOrderFiles = _playOrderService.Create(previousFile, PlaylistCollection.Current.Directories, Shuffle);
            }

            PlayOrderCurrentFile = previousFile;
        }

        public void Initialize(PlaylistFileViewModel playlistFileViewModel)
        {
            _musicPlayerService.Initialize(playlistFileViewModel.Type, playlistFileViewModel.Stream);
        }

        public void Play()
        {
            if (ShouldStop)
            {
                return;
            }
            _musicPlayerService.Play();
        }

        public void Stop()
        {
            _musicPlayerService.Stop();
        }

        public void Resume()
        {
            _musicPlayerService.Resume();
        }

        public void Pause()
        {
            _musicPlayerService.Pause();
        }

        public void AddFilesCommandAction()
        {
            var remoteFileDialogView = new RemoteFileDialog();
            remoteFileDialogView.ShowDialog();
            if (remoteFileDialogView.DialogResult != true)
            {
                return;
            }

            var selectedFiles = remoteFileDialogView.SelectedFiles;

            _playlistService.Add(PlaylistCollection.Selected, selectedFiles);
        }

        public void CurrentPlaylistChanged(object sender, EventArgs eventArgs)
        {
            var currentFile = PlayOrderCurrentFile;
            PlayOrderFiles = _playOrderService.Create(currentFile, PlaylistCollection.Current.Directories, Shuffle);
            PlayOrderCurrentFile = currentFile;
        }

        public DelegateCommand AddFilesCommand { get; }

        public IPlaylistCollectionViewModel PlaylistCollection { get; }

        private int PlayOrderCurrentFileIndex
        {
            get => _playOrderCurrentFileIndex;
            set
            {
                if (Equals(_playOrderCurrentFileIndex, value))
                {
                    return;
                }
                _playOrderCurrentFileIndex = value;
                RaisePropertyChanged(nameof(PlayOrderCurrentFile));
                RaisePropertyChanged(nameof(PlayOrderNextFile));
                RaisePropertyChanged(nameof(PlayOrderPreviousFile));
            }
        }

        public PlaylistFileViewModel PlayOrderCurrentFile
        {
            get => PlayOrderFiles[PlayOrderCurrentFileIndex];
            set
            {
                PlayOrderCurrentFileIndex = PlayOrderFiles.IndexOf(value);
                try
                {
                    Initialize(value);
                    Play();
                    _onlineStatusService.BecomeOnline();
                    _playlistFileDownloadStateService.AddToDownloaded(value);
                }
                catch (ArgumentException)
                {
                    PlaylistCollection.Current.Remove(value);
                    PlayNext();
                }
                catch (Exception exception)
                {
                    if (exception is IOException || exception is HttpRequestException)
                    {
                        _onlineStatusService.BecomeOffline();
                        PlayNext();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
        }

        public PlaylistFileViewModel PlayOrderNextFile =>
            PlayOrderCurrentFileIndex != PlayOrderFiles.Count - 1
                ? PlayOrderFiles[PlayOrderCurrentFileIndex + 1]
                : (RepeatPlaylist
                    ? PlayOrderFiles[0]
                    : PlaylistCollection.Next.First);

        public PlaylistFileViewModel PlayOrderPreviousFile =>
            PlayOrderCurrentFileIndex != 0
                ? PlayOrderFiles[PlayOrderCurrentFileIndex - 1]
                : (RepeatPlaylist
                    ? PlayOrderFiles.Last()
                    : PlaylistCollection.Previous.Last);


        public List<PlaylistFileViewModel> PlayOrderFiles { get; set; }

        public PlaylistFileViewModel SelectedFile
        {
            get => _selectedFile;
            set
            {
                if (_selectedFile != null)
                {
                    _selectedFile.IsSelected = false;
                }
                _selectedFile = value;
                if (_selectedFile != null)
                {
                    _selectedFile.IsSelected = true;
                }
            }
        }

        public bool RepeatTrack
        {
            get => _repeatTrack;
            set => SetProperty(ref _repeatTrack, value);
        }

        public bool RepeatPlaylist
        {
            get => _repeatPlaylist;
            set => SetProperty(ref _repeatPlaylist, value);
        }

        public bool StopOnPlaylistEnd
        {
            get => _stopOnPlaylistEnd;
            set => SetProperty(ref _stopOnPlaylistEnd, value);
        }

        public bool Shuffle
        {
            get => _shuffle;
            set => SetProperty(ref _shuffle, value);
        }

        public OnlineStatus CurrentOnlineStatus
        {
            get => _currentOnlineStatus;
            set => SetProperty(ref _currentOnlineStatus, value);
        }

        /// <summary>
        /// Determines whether play should top due to absent downloaded files in PlayOrder
        /// </summary>
        public bool ShouldStop
        {
            get
            {
                if (RepeatTrack)
                {
                    return _playlistFileDownloadStateService.ContainsDownloaded(PlayOrderCurrentFile);
                }
                if (StopOnPlaylistEnd)
                {
                    return _playlistFileDownloadStateService.ContainsDownloaded(
                        PlayOrderFiles.Skip(PlayOrderCurrentFileIndex + 1));
                }
                if (RepeatPlaylist)
                {
                    return _playlistFileDownloadStateService.ContainsDownloaded(PlayOrderFiles);
                }
                return _playlistFileDownloadStateService.ContainsDownloaded(PlaylistCollection.AllFiles);
            }
        }
    }
}
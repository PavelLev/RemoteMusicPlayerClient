﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using RemoteMusicPlayerClient.DryIoc;
using RemoteMusicPlayerClient.Music.Playlisting.Collection;
using RemoteMusicPlayerClient.Networking;
using RemoteMusicPlayerClient.Networking.Files;

namespace RemoteMusicPlayerClient.Music.Playlisting
{
    class PlaylistService : IPlaylistService
    {
        private readonly IFileService _fileService;
        private readonly IResolver _resolver;
        private readonly IMetadataService _metadataService;
        private readonly IPlaylistSaverService _playlistSaverService;
        private readonly IOnlineStatusService _onlineStatusService;

        public PlaylistService(IFileService fileService, IResolver resolver, IMetadataService metadataService,
            IPlaylistSaverService playlistSaverService, IOnlineStatusService onlineStatusService,
            IPlaylistCollectionViewModel playlistCollectionViewModel)
        {
            _fileService = fileService;
            _resolver = resolver;
            _metadataService = metadataService;
            _playlistSaverService = playlistSaverService;
            _onlineStatusService = onlineStatusService;

            onlineStatusService.OnlineStatusChanged += newOnlineStatus =>
            {
                if (newOnlineStatus == OnlineStatus.Offline)
                {
                    return;
                }

                foreach (var playlistViewModel in playlistCollectionViewModel.All)
                {
                    LoadMetadata(playlistViewModel);
                }
            };
        }

        public void AddSources(ObservableCollection<string> sourceDirectories, IEnumerable<string> newSourceDirectories)
        {
            newSourceDirectories = newSourceDirectories.Where(sourceDirectories.Contains);

            var updatedSourceDirectories = sourceDirectories.Concat(newSourceDirectories).ToList();
            updatedSourceDirectories.Sort();

            sourceDirectories.Clear();
            updatedSourceDirectories.ForEach(sourceDirectories.Add);
        }

        public void Rescan(PlaylistViewModel playlistViewModel)
        {
            var files = playlistViewModel.SourceDirectories.Select(async sourceDirectory =>
            {
                try
                {
                    return await _fileService.GetAllFilesAsync(sourceDirectory);
                }
                catch (HttpException httpException)
                {
                    if (httpException.GetHttpCode() == (int)HttpStatusCode.BadRequest)
                    {
                        playlistViewModel.SourceDirectories.Remove(sourceDirectory);
                        return Enumerable.Empty<string>();
                    }
                    throw;
                }
            }).SelectMany(task => task.Result);

            var directories = files.GroupBy(filePath => filePath.Substring(0, filePath.LastIndexOf('\\') + 1))
                .OrderBy(grouping => grouping.Key)
                .Select(grouping => new PlaylistDirectoryViewModel
                {
                    Name = grouping.Key,
                    Files = new ObservableCollection<PlaylistFileViewModel>(grouping.Select(
                        _resolver.Resolve<Func<string, PlaylistFileViewModel>>()))
                });
            playlistViewModel.Directories = new ObservableCollection<PlaylistDirectoryViewModel>(directories);

            LoadMetadata(playlistViewModel);
        }

        public async void LoadMetadata(PlaylistViewModel playlistViewModel)
        {
            foreach (var playlistFileViewModel in playlistViewModel.AllFiles)
            {
                if (playlistFileViewModel.Metadata != null)
                {
                    continue;
                }
                try
                {
                    playlistFileViewModel.Metadata = await _metadataService.GetAsync(playlistFileViewModel.Path);
                }
                catch (HttpException httpException)
                {
                    if (httpException.GetHttpCode() != (int) HttpStatusCode.BadRequest)
                    {
                        throw;
                    }
                }
                catch (HttpRequestException)
                {
                    _onlineStatusService.BecomeOffline();
                    _playlistSaverService.SaveAll();
                    return;
                }
            }
            _onlineStatusService.BecomeOnline();
            _playlistSaverService.SaveAll();
        }
    }
}
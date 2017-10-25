using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Web;
using RemoteMusicPlayerClient.DryIoc;
using RemoteMusicPlayerClient.Networking.Files;

namespace RemoteMusicPlayerClient.Music.Playlisting
{
    class PlaylistService : IPlaylistService
    {
        private readonly IFileService _fileService;
        private readonly IResolver _resolver;
        private readonly IMetadataService _metadataService;

        public PlaylistService(IFileService fileService, IResolver resolver, IMetadataService metadataService)
        {
            _fileService = fileService;
            _resolver = resolver;
            _metadataService = metadataService;
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

            LoadMetadata(playlistViewModel.AllFiles);
        }

        public async void LoadMetadata(IEnumerable<PlaylistFileViewModel> files)
        {
            foreach (var playlistFileViewModel in files)
            {
                try
                {
                    playlistFileViewModel.Metadata = await _metadataService.GetAsync(playlistFileViewModel.Path);
                }
                catch (HttpException httpException)
                {
                    if (httpException.GetHttpCode() != (int)HttpStatusCode.BadRequest)
                    {
                        throw;
                    }
                }
            }
        }
    }
}
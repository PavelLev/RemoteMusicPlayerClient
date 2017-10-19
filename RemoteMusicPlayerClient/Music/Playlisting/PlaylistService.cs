using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using RemoteMusicPlayerClient.CustomFrameworkElements.DryIoc;

namespace RemoteMusicPlayerClient.Music.Playlisting
{
    public class PlaylistService : IPlaylistService
    {
        private readonly IContainer _container;

        public PlaylistService(IContainer container)
        {
            _container = container;
        }

        public void Add(PlaylistViewModel playlist, List<string> files)
        {
            var directories = files.GroupBy(filePath => filePath.Substring(0, filePath.LastIndexOf('\\') + 1))
                .OrderBy(grouping => grouping.Key)
                .Select(grouping => new PlaylistDirectoryViewModel
                {
                    Name = grouping.Key,
                    Files = new ObservableCollection<PlaylistFileViewModel>(grouping.Select(
                        _container.Resolve<Func<string, PlaylistFileViewModel>>()))
                });
            playlist.Directories = new ObservableCollection<PlaylistDirectoryViewModel>(directories);
        }
    }
}
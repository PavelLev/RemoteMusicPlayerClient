using System;
using System.Collections.Generic;
using System.Linq;
using RemoteMusicPlayerClient.DryIoc;
using RemoteMusicPlayerClient.Music.Playlisting;
using RemoteMusicPlayerClient.Utility;

namespace RemoteMusicPlayerClient.Music
{
    class PlayOrderService : IPlayOrderService
    {
        private readonly IContainer _container;

        public PlayOrderService(IContainer container)
        {
            _container = container;
        }

        public List<PlaylistFileViewModel> Create(PlaylistFileViewModel firstFile, IEnumerable<PlaylistDirectoryViewModel> directories, bool shuffle)
        {
            var files = directories.SelectMany(directory => directory.Files).ToList();

            if (shuffle)
            {
                files = files.Shuffle().ToList();

                var index = files.IndexOf(firstFile);

                var temp = files[0];
                files[0] = files[index];
                files[index] = temp;
            }

            return files;
        }
    }
}
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RemoteMusicPlayerClient.Music.Playlisting
{
    public class PlaylistSaverService : IPlaylistSaverService
    {
        private readonly IPlaylistCollectionViewModel _playlistCollectionViewModel;
        private readonly JsonSerializer _jsonSerializer;
        private readonly string _playlistsFilePath = "./playlists.json";
        private Task _savingTask = Task.Run(() => { });

        public PlaylistSaverService(IPlaylistCollectionViewModel playlistCollectionViewModel, JsonSerializer jsonSerializer)
        {
            _playlistCollectionViewModel = playlistCollectionViewModel;
            _jsonSerializer = jsonSerializer;
        }

        public void SaveAll()
        {
            _savingTask = _savingTask.ContinueWith(task =>
            {
                using (var streamWriter = new StreamWriter(_playlistsFilePath))
                using (var jsonTextWriter = new JsonTextWriter(streamWriter))
                {
                    _jsonSerializer.Serialize(jsonTextWriter, _playlistCollectionViewModel.All);
                }
            });
        }

        public T LoadAll<T>() where T : IEnumerable<PlaylistViewModel>
        {
            using (var streamReader = new StreamReader(_playlistsFilePath))
            using (var jsonTextReader = new JsonTextReader(streamReader))
            {
                return _jsonSerializer.Deserialize<T>(jsonTextReader);
            }
        }
    }
}
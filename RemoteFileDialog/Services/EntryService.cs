using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using RemoteFileDialog.Entries;

namespace RemoteFileDialog.Services
{
    public class EntryService : IEntryService
    {
        private readonly JsonSerializer _serializer;
        private readonly HttpClient _httpClient;
        private readonly string _getChildEntriesUrl = "http://localhost:38769/FileSystem/GetChildEntries";

        public EntryService(JsonSerializer serializer, HttpClient httpClient)
        {
            _serializer = serializer;
            _httpClient = httpClient;
        }


        public async Task<IEnumerable<Entry>> LoadChildEntriesAsync(string path, bool recursive = false)
        {
            var query = HttpUtility.ParseQueryString("");
            query["path"] = path;
            query["recursive"] = recursive.ToString();
            var httpResponse = await _httpClient.GetAsync($"{_getChildEntriesUrl}?{query}");

            using (var stream = await httpResponse.Content.ReadAsStreamAsync())
            using (var streamReader = new StreamReader(stream))
            using (var jsonTextReader = new JsonTextReader(streamReader))
            {
                return _serializer.Deserialize<IEnumerable<Entry>>(jsonTextReader);
            }
            
        }

        public IEnumerable<Entry> LoadRootEntries()
        {
            throw new System.NotImplementedException();
        }

        public bool EntryExists(string path)
        {
            throw new System.NotImplementedException();
        }
    }
}
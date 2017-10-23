using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;

namespace RemoteMusicPlayerClient.CustomFrameworkElements.RemoteFileDialog.Entries
{
    public class EntryService : IEntryService
    {
        private readonly JsonSerializer _serializer;
        private readonly HttpClient _httpClient;
        private readonly IDialogModeService _dialogModeService;
        private readonly string _getChildEntriesUrl = "http://localhost:38769/FileSystem/GetChildEntries";
        private readonly string _getChildDirectoriesUrl = "http://localhost:38769/FileSystem/GetChildDirectories";
        private readonly string _getRootEntriesUrl = "http://localhost:38769/FileSystem/GetRootEntries";
        private readonly string _getEntryExistsUrl = "http://localhost:38769/FileSystem/EntryExists";

        public EntryService(JsonSerializer serializer, HttpClient httpClient, IDialogModeService dialogModeService)
        {
            _serializer = serializer;
            _httpClient = httpClient;
            _dialogModeService = dialogModeService;
        }


        public async Task<IEnumerable<Entry>> GetChildEntriesAsync(string path, bool recursive = false)
        {
            switch (_dialogModeService.Current)
            {
                case DialogMode.Files:
                    return await GetAsync<IEnumerable<Entry>>(_getChildEntriesUrl, new Dictionary<string, string>
                        {
                            {"path", path},
                            {"recursive", recursive.ToString()},
                        });
                case DialogMode.Directories:
                    return await GetAsync<IEnumerable<Entry>>(_getChildDirectoriesUrl, new Dictionary<string, string>
                        {
                            {"path", path}
                        });
                default:
                    throw new InvalidOperationException("Unsupported DialogMode");
            }

        }

        public async Task<IEnumerable<Entry>> GetRootEntriesAsync()
        {
            return await GetAsync<IEnumerable<Entry>>(_getRootEntriesUrl);
        }

        public async Task<bool> EntryExists(string path)
        {
            return await GetAsync<bool>(_getEntryExistsUrl, new Dictionary<string, string>
            {
                {"path", path},
            });
        }

        private async Task<T> GetAsync<T>(string url, Dictionary<string, string> arguments = null)
        {
            if (arguments != null && arguments.Count > 0)
            {
                var query = HttpUtility.ParseQueryString("");
                foreach (var argument in arguments)
                {
                    query[argument.Key] = argument.Value;
                }
                url = $"{url}?{query}";
            }

            var httpResponse = await _httpClient.GetAsync(url);

            using (var stream = await httpResponse.Content.ReadAsStreamAsync())
            using (var streamReader = new StreamReader(stream))
            using (var jsonTextReader = new JsonTextReader(streamReader))
            {
                return _serializer.Deserialize<T>(jsonTextReader);
            }
        }
    }
}
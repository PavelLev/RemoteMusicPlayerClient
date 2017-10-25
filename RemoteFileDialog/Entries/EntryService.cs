using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using RemoteMusicPlayerClient.CustomFrameworkElements.RemoteFileDialog.Utility;

namespace RemoteMusicPlayerClient.CustomFrameworkElements.RemoteFileDialog.Entries
{
    public class EntryService : BaseHttpService, IEntryService
    {
        private readonly IDialogModeService _dialogModeService;
        private readonly string _getChildEntriesUrl = "http://localhost:38769/FileSystem/GetChildEntries";
        private readonly string _getChildDirectoriesUrl = "http://localhost:38769/FileSystem/GetChildDirectories";
        private readonly string _getRootEntriesUrl = "http://localhost:38769/FileSystem/GetRootEntries";
        private readonly string _getEntryExistsUrl = "http://localhost:38769/FileSystem/EntryExists";

        public EntryService(JsonSerializer serializer, HttpClient httpClient, IDialogModeService dialogModeService) : base(serializer, httpClient)
        {
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
    }
}
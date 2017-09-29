using System.Net.Http;
using System.Web;
using RemoteFileDialog.Entries;

namespace RemoteFileDialog.Services
{
    public class EntryService : IEntryService
    {
        private HttpClient _httpClient = new HttpClient();
        private readonly string _getChildEntriesUrl = "http://localhost:38769/FileSystem/GetChildEntries";


        public void LoadChildEntries(IEntry entry)
        {
            var query = HttpUtility.ParseQueryString("");
            query["path"] = entry.Path;
            _httpClient.GetAsync($"{_getChildEntriesUrl}?{query}");
        }

        public void LoadChildEntriesRecursively(IEntry entry)
        {
            throw new System.NotImplementedException();
        }
    }
}
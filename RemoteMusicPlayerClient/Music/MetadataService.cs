using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RemoteMusicPlayerClient.Utility;

namespace RemoteMusicPlayerClient.Music
{
    public class MetadataService : BaseHttpService, IMetadataService
    {
        private readonly string _getMetadata = "http://localhost:38769/FileSystem/GetMetadata";

        public MetadataService(JsonSerializer serializer, HttpClient httpClient) : base(serializer, httpClient)
        {
        }

        public async Task<Metadata> GetAsync(string filePath)
        {
            return await GetAsync<Metadata>(_getMetadata, new Dictionary<string, string>
            {
                {nameof(filePath), filePath},
            });
        }
    }
}
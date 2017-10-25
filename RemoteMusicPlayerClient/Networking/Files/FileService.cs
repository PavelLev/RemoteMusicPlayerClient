using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using RemoteMusicPlayerClient.Utility;

namespace RemoteMusicPlayerClient.Networking.Files
{
    public class FileService : BaseHttpService, IFileService
    {
        private readonly string _getAllChildFiles = "http://localhost:38769/FileSystem/GetAllChildFiles";

        public FileService(JsonSerializer serializer, HttpClient httpClient) : base(serializer, httpClient)
        {

        }

        public async Task<List<string>> GetAllFilesAsync(string path)
        {
            return await GetAsync<List<string>>(_getAllChildFiles, new Dictionary<string, string>
                {
                    {"path", path},
                });
        }
    }
}
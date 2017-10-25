using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using RemoteMusicPlayerClient.DryIoc;
using RemoteMusicPlayerServer.Model;

namespace RemoteMusicPlayerClient.Networking
{
    public class RemoteFileReaderFactory : IRemoteFileReaderFactory
    {
        private readonly IContainer _container;
        private readonly HttpClient _httpClient;
        private readonly SHA256 _sha256;
        private readonly Dictionary<string, FileUsage> _fileUsages = new Dictionary<string, FileUsage>();

        public RemoteFileReaderFactory(IContainer container, HttpClient httpClient, SHA256 sha256)
        {
            _container = container;
            _httpClient = httpClient;
            _sha256 = sha256;
        }

        public async Task<RemoteFileReader> StartUse(string filePath)
        {
            if (_fileUsages.ContainsKey(filePath))
            {
                _fileUsages[filePath].Usages++;
            }
            else
            {
                _fileUsages[filePath] = new FileUsage
                {
                    Stream = await ByPath(filePath),
                    Usages = 1
                };
            }

            return _fileUsages[filePath].Stream;
        }

        public void StopUse(string filePath)
        {
            _fileUsages[filePath].Usages--;
            if (_fileUsages[filePath].Usages == 0)
            {
                _fileUsages[filePath].Stream.Close();
                _fileUsages.Remove(filePath);
            }
        }

        public async Task<RemoteFileReader> ByPath(string filePath)
        {
            var query = HttpUtility.ParseQueryString("");
            query["filePath"] = filePath;
            var url = $"http://localhost:38769/filesystem/GetTokenForFile?{query}";

            var httpResponseMessage = await _httpClient.GetAsync(url);

            var responseContent = await httpResponseMessage.Content.ReadAsStringAsync();

            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                throw new ArgumentException("filePath is incorrect");
            }

            switch (httpResponseMessage.StatusCode)
            {
                case HttpStatusCode.OK:
                    break;
                case HttpStatusCode.NotFound:
                    throw new ArgumentException("file doesn't exist", nameof(filePath));
                case HttpStatusCode.Conflict:
                    throw new ArgumentException("file has wrong extension", nameof(filePath));
                default:
                    throw new InvalidOperationException("Unsupported StatusCode");
            }

            var fileTokenAndLength = JsonConvert.DeserializeObject<FileTokenAndLength>(responseContent);

            var cacheFileName = GetCacheFileName(filePath);

            return _container.Resolve<Func<long, string, string, RemoteFileReader>>()(fileTokenAndLength.Length,
                fileTokenAndLength.Token, cacheFileName);
        }

        private string GetCacheFileName(string path)
        {
            var hash = _sha256.ComputeHash(Encoding.UTF8.GetBytes(path));

            var base64 = Convert.ToBase64String(hash).Replace('/', '-');

            return base64;
        }
    }
}
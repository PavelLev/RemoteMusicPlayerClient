using System;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Web;
using RemoteMusicPlayerClient.DryIoc;

namespace RemoteMusicPlayerClient.Networking
{
    public class RemoteFileReaderFactory : IRemoteFileReaderFactory
    {
        private readonly IContainer _container;

        public RemoteFileReaderFactory(IContainer container)
        {
            _container = container;
        }

        public async Task<RemoteFileReader> ByPath(string path)
        {
            var token = await GetTokenByPath(path);
            return await ByToken(token);
        }

        public async Task<string> GetTokenByPath(string path)
        {
            var httpClient = new HttpClient();

            var query = HttpUtility.ParseQueryString("");
            query["filePath"] = path;
            var url = $"http://localhost:38769/filesystem/GetTokenForFile?{query}";
            
            var httpResponseMessage = await httpClient.GetAsync(url);

            var responseContent = await httpResponseMessage.Content.ReadAsStringAsync();

            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                throw new ArgumentException("path is incorrect");
            }

            switch (httpResponseMessage.StatusCode)
            {
                case HttpStatusCode.OK:
                    break;
                case HttpStatusCode.NotFound:
                    throw new ArgumentException("file doesn't exist", nameof(path));
                case HttpStatusCode.Conflict:
                    throw new ArgumentException("file has wrong extension", nameof(path));
                default:
                    throw new InvalidOperationException("Unsupported StatusCode");
            }
            return responseContent;
        }

        public async Task<RemoteFileReader> ByToken(string token)
        {
            var tcpClient = new TcpClient();

            await tcpClient.ConnectAsync("localhost", 54364);

            var networkStream = tcpClient.GetStream();

            networkStream.WriteAsync(token);

            var intBuffer = new byte[4];
            var result = await networkStream.ReadAsync(intBuffer, 0, 4);
            if (result != 4)
            {
                throw new NetworkInformationException();
            }
            var length = BitConverter.ToInt32(intBuffer, 0);

            return _container.Resolve<Func<int, NetworkStream, RemoteFileReader>>()(length, networkStream);
        }
    }
}
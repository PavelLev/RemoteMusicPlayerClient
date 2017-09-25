using System;
using System.Net.Http;
using System.Net.Sockets;
using System.Web;
using RemoteMusicPlayerClient.Services;
using RemoteMusicPlayerClient.Utility;

namespace RemoteMusicPlayerClient
{
    public class Junk
    {
        private static readonly string _filePath = "D:\\Offtop\\Music\\Mr. Robot OST\\Mac Quayle - Mr. Robot, Vol. 1 (2016)\\08. 1.0_8-whatsyourask.m4p.flac";

        public static async void DoShit()
        {
            var httpClient = new HttpClient();

            
            var query = HttpUtility.ParseQueryString("");
            query["filePath"] = _filePath;
            var url = $"http://localhost:5000/filesystem/GetTokenForFile?{query}";


            var httpResponseMessage = await httpClient.GetAsync(url);

            var token = await httpResponseMessage.Content.ReadAsStringAsync();

            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                throw new Exception("Plohoy zapros " + token);
            }



            var tcpClient = new TcpClient();

            await tcpClient.ConnectAsync("localhost", 54364);

            var networkStream = tcpClient.GetStream();

            networkStream.WriteAsync(token);

            MusicPlayer.Instance.Initialize(FileTypeHelper.Instance.GetFileType(_filePath), networkStream);

            MusicPlayer.Instance.Play();
        }
    }
}
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace RemoteMusicPlayerClient
{
    public static class NetworkStreamExtensions
    {
        public static async Task<string> ReadAsync(this NetworkStream networkStream)
        {
            var buffer = new byte[256];
            var result = await networkStream.ReadAsync(buffer, 0, buffer.Length);
            return Encoding.UTF8.GetString(buffer, 0, result);
        }

        public static void WriteAsync(this NetworkStream networkStream, string value)
        {
            var buffer = Encoding.UTF8.GetBytes(value);
            networkStream.WriteAsync(buffer, 0, buffer.Length);
        }
    }
}
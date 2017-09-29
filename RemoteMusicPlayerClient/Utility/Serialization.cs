using Newtonsoft.Json;

namespace RemoteMusicPlayerClient.Utility
{
    public class Serialization
    {
        public static JsonSerializer Serializer { get; } = new JsonSerializer();
    }
}
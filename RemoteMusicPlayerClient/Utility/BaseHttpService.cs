using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;

namespace RemoteMusicPlayerClient.Utility
{
    public abstract class BaseHttpService
    {
        private readonly JsonSerializer _serializer;
        private readonly HttpClient _httpClient;

        protected BaseHttpService(JsonSerializer serializer, HttpClient httpClient)
        {
            _serializer = serializer;
            _httpClient = httpClient;
        }

        protected async Task<T> GetAsync<T>(string url, Dictionary<string, string> arguments = null)
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

            if (!httpResponse.IsSuccessStatusCode)
            {
                throw new HttpException((int)httpResponse.StatusCode, $"Server responded with code {httpResponse.StatusCode}");
            }

            using (var stream = await httpResponse.Content.ReadAsStreamAsync())
            using (var streamReader = new StreamReader(stream))
            using (var jsonTextReader = new JsonTextReader(streamReader))
            {
                return _serializer.Deserialize<T>(jsonTextReader);
            }
        }
    }
}
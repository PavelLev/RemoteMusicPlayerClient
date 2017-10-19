using RemoteMusicPlayerClient.Music;

namespace RemoteMusicPlayerClient.Utility
{
    public class FileNameExtractor : IFileNameExtractor
    {
        public string GetName(string path)
        {
            var beginIndex = path.LastIndexOf('\\') + 1;

            var endIndex = path.LastIndexOf('.') - 1;

            return path.Substring(beginIndex, endIndex - beginIndex + 1);
        }
    }
}
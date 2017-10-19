using System.IO;
using CSCore;

namespace RemoteMusicPlayerClient.Utility
{
    public interface IFileTypeHelper
    {
        IWaveSource Decode(FileType fileType, Stream stream);
        FileType GetFileType(string filePath);
    }
}
using System;
using System.IO;
using CSCore;
using CSCore.Codecs.FLAC;
using CSCore.Codecs.MP3;

namespace RemoteMusicPlayerClient.Utility
{
    public class FileTypeHelper
    {
        public IWaveSource Decode(FileType fileType, Stream stream)
        {
            switch (fileType)
            {
                case FileType.Flac:
                    return new FlacFile(stream);
                case FileType.Mp3:
                    return new DmoMp3Decoder(stream);
                default:
                    throw new ArgumentException($"File type {fileType} is not recognized");
            }
        }

        public FileType GetFileType(string filePath)
        {
            if (filePath.EndsWith(".flac"))
            {
                return FileType.Flac;
            }
            if (filePath.EndsWith(".mp3"))
            {
                return FileType.Mp3;
            }

            throw new ArgumentException("Unknown file type");
        }

        public static FileTypeHelper Instance { get; } = new FileTypeHelper();
    }
}
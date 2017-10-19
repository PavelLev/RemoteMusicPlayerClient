using System;
using System.IO;
using CSCore;
using RemoteMusicPlayerClient.Utility;

namespace RemoteMusicPlayerClient.Music
{
    public interface IMusicPlayerService
    {
        void Initialize(FileType fileType, Stream stream);
        void Initialize(IWaveSource waveSource);
        void Play();
        void Stop();
        void Resume();
        void Pause();

        event Action Stopped;

        float Volume { get; set; }
        long Length { get; }
        long Position { get; set; }
    }
}
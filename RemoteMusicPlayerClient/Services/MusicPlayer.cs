using System.IO;
using CSCore;
using CSCore.Codecs;
using CSCore.SoundOut;
using CSCore.Streams;
using CSCore.Streams.Effects;
using CSCore.XAudio2;
using RemoteMusicPlayerClient.Utility;

namespace RemoteMusicPlayerClient.Services
{
    public class MusicPlayer
    {
        private readonly ISoundOut _soundOut = new WasapiOut();

        public void Initialize(FileType fileType, Stream stream)
        {
            var waveSource = FileTypeHelper.Instance.Decode(fileType, stream);

            Initialize(waveSource);
        }

        public void Initialize(IWaveSource waveSource)
        {
            _soundOut.Initialize(waveSource);
            _soundOut.Volume = Volume;
        }

        public void Play()
        {
            _soundOut.Play();
        }

        public void Stop()
        {
            _soundOut.Stop();
        }

        public void Resume()
        {
            _soundOut.Resume();
        }

        public void Pause()
        {
            _soundOut.Pause();
        }

        public float Volume { get; set; } = 0.02f;

        public long Length => _soundOut.WaveSource.Length;

        public long Position
        {
            get => _soundOut.WaveSource.Position;
            set => _soundOut.WaveSource.Position = value;
        }

        public static MusicPlayer Instance { get; } = new MusicPlayer();
    }
}
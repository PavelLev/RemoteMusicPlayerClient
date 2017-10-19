using System;
using System.IO;
using System.Windows;
using CSCore;
using CSCore.SoundOut;
using RemoteMusicPlayerClient.Utility;

namespace RemoteMusicPlayerClient.Music
{
    public class MusicPlayerService : IMusicPlayerService
    {
        private readonly ISoundOut _soundOut;
        private readonly IFileTypeHelper _fileTypeHelper;

        private bool _isManuallyStopped;

        public event Action Stopped;

        public MusicPlayerService(ISoundOut soundOut, IFileTypeHelper fileTypeHelper)
        {
            _soundOut = soundOut;
            _fileTypeHelper = fileTypeHelper;
            _soundOut.Stopped += _soundOutStopped;
        }

        private void _soundOutStopped(object sender, PlaybackStoppedEventArgs playbackStoppedEventArgs)
        {
            Position = 0;

            if (_isManuallyStopped)
            {
                _isManuallyStopped = false;
                Stopped?.Invoke();
            }
        }

        public void Initialize(FileType fileType, Stream stream)
        {
            var waveSource = _fileTypeHelper.Decode(fileType, stream);
            
            Initialize(waveSource);
        }

        public void Initialize(IWaveSource waveSource)
        {
            if (_soundOut.PlaybackState != PlaybackState.Stopped)
            {
                Stop();
            }
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
            _isManuallyStopped = true;
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
    }
}
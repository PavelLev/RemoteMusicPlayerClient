using System;
using System.IO;
using System.Threading.Tasks;
using RemoteMusicPlayerClient.Utility.Segments;

namespace RemoteMusicPlayerClient.Networking.Caching
{
    public class CachingService : ICachingService
    {
        private Task _cachingTask;
        private byte[] _buffer;
        private FileStream _writeCacheFileStream;

        public Task<int> InitializeAsync(string filePath, byte[] buffer)
        {
            if (_buffer != null)
            {
                throw new InvalidOperationException();    
            }

            _buffer = buffer;

            var readFileStream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Read, FileShare.None, 4096, true);

            var readTask = readFileStream.ReadAsync(_buffer, 0, (int)readFileStream.Length);
            _cachingTask = readTask.ContinueWith(task =>
            {
                readFileStream.Close();

                _writeCacheFileStream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None, 4096, true);
            });
            return readTask;
        }

        public void Write(Segment segment)
        {
            _cachingTask = _cachingTask.ContinueWith(cachingTask =>
            {
                _writeCacheFileStream.Seek(segment.Begin, SeekOrigin.Begin);
                _writeCacheFileStream.Write(_buffer, segment.Begin, segment.Count);
            });
        }

        public void Close()
        {
            _cachingTask.ContinueWith(task =>
            {
                _writeCacheFileStream.Close();
            });
        }
    }
}
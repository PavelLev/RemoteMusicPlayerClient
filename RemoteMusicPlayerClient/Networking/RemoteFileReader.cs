using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RemoteMusicPlayerClient.Networking.Caching;
using RemoteMusicPlayerClient.Utility.Segments;

namespace RemoteMusicPlayerClient.Networking
{
    public class RemoteFileReader: Stream
    {
        private const int MemStreamMaxLength = int.MaxValue;
        private int _position;
        private readonly int _length;
        private readonly string _token;
        private bool _isOpen = true;
        private readonly byte[] _buffer;
        private readonly SegmentCollection _localSegments;
        private NetworkStream _networkStream;
        private readonly JsonSerializer _jsonSerializer;
        private readonly IOnlineStatusService _onlineStatusService;
        private readonly ICachingService _cachingService;
        private JsonTextWriter _jsonTextWriter;
        private bool _shouldReconnect;

        public RemoteFileReader(int length, string token, string cacheFileName, JsonSerializer jsonSerializer, IOnlineStatusService onlineStatusService, ICachingService cachingService)
        {
            _length = length;
            _token = token;

            _localSegments = new SegmentCollection(length);

            _buffer = new byte[length];
            
            _jsonSerializer = jsonSerializer;
            _onlineStatusService = onlineStatusService;
            _cachingService = cachingService;

            InitializeNetworkStream().Wait();

            onlineStatusService.OnlineStatusChanged += newOnlineStatus =>
            {
                if (newOnlineStatus == OnlineStatus.Online)
                {
                    _shouldReconnect = true;
                }
            };

            _cachingService.InitializeAsync(cacheFileName, _buffer)
                .ContinueWith(readTask => _localSegments.Add(new Segment(0, readTask.Result)));
        }

        private async Task InitializeNetworkStream()
        {
            var tcpClient = new TcpClient();
            await tcpClient.ConnectAsync("localhost", 54364);

            var networkStream = tcpClient.GetStream();

            networkStream.WriteAsync(_token);

            _networkStream = networkStream;
            _jsonTextWriter = new JsonTextWriter(new StreamWriter(networkStream));
        }

        public override void Flush()
        {
            throw new NotImplementedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            ThrowIfStreamIsClosed();

            EnsureValueIsCorrect(offset);


            switch (origin)
            {
                case SeekOrigin.Begin:
                {
                    var tempPosition = unchecked((int)offset);
                    _position = tempPosition;
                    break;
                }
                case SeekOrigin.Current:
                {
                    var tempPosition = unchecked(_position + (int)offset);
                    if (unchecked(_position + offset) < 0 || tempPosition < 0)
                        throw new IOException("IO.IO_SeekBeforeBegin");
                    _position = tempPosition;
                    break;
                }
                case SeekOrigin.End:
                {
                    var tempPosition = unchecked(_length + (int)offset);
                    if (unchecked(_length + offset) < 0 || tempPosition < 0)
                        throw new IOException("IO.IO_SeekBeforeBegin");
                    _position = tempPosition;
                    break;
                }
                default:
                {
                    throw new ArgumentException("Argument_InvalidSeekOrigin");
                }
            }

            Contract.Assert(_position >= 0, "_position >= 0");
            return _position;
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public int Number { get; set; } = 0;

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer), "Buffer shouldn't be null");
            }
            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(offset), "Offset shouldn't be negative");
            }
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count), "Count shouldn't be negative");
            }
            if (offset + count > buffer.Length)
            {
                throw new ArgumentException("Requested segment exceeds file size");
            }
            ThrowIfStreamIsClosed();

            var numberOfBytesToRead = _length - _position;
            if (numberOfBytesToRead > count)
            {
                numberOfBytesToRead = count;
            }
            if (numberOfBytesToRead <= 0)
            {
                return 0;
            }
            
            var absentSegments = _localSegments.Add(new Segment(_position, _position + numberOfBytesToRead - 1));
            
            if (_networkStream == null && _shouldReconnect)
            {
                _shouldReconnect = false;
                try
                {
                    InitializeNetworkStream().Wait();
                }
                catch (IOException)
                {
                    _onlineStatusService.BecomeOffline();
                    return 0;
                }
            }

            if (_networkStream == null)
            {
                return 0;
            }

            var readTasks = absentSegments.Select(absentSegment =>
            {
                _jsonSerializer.Serialize(_jsonTextWriter, absentSegment);
                _jsonTextWriter.Flush();

                var readTask = _networkStream.ReadAsync(_buffer, absentSegment.Begin, absentSegment.Count);

                readTask.ContinueWith(task =>
                {
                    _cachingService.Write(absentSegment);
                });

                return readTask;
            }).ToArray();

            try
            {
                Task.WaitAll(readTasks);

                _onlineStatusService.BecomeOnline();
            }
            catch (Exception exception)
            {
                if (exception is IOException || exception is SocketException)
                {
                    _networkStream.Close();
                    _networkStream = null;

                    _onlineStatusService.BecomeOffline();
                    
                    return 0;
                }
                throw;
            }

            Array.Copy(_buffer, _position, buffer, offset, numberOfBytesToRead);
            _position += numberOfBytesToRead;

            return numberOfBytesToRead;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        public override void Close()
        {
            if (!_isOpen)
            {
                return;
            }
            _isOpen = false;
            _networkStream.Close();

            _cachingService.Close();

            base.Close();
        }

        public override bool CanRead { get; } = true;
        public override bool CanSeek { get; } = true;
        public override bool CanWrite { get; } = false;

        public override long Length
        {
            get
            {
                ThrowIfStreamIsClosed();
                return _length;
            }
        }

        public override long Position
        {
            get
            {
                ThrowIfStreamIsClosed();
                return _position;
            }
            set
            {
                ThrowIfStreamIsClosed();

                EnsureValueIsCorrect(value);

                _position = (int)value;
            }
        }

        private void ThrowIfStreamIsClosed()
        {
            if (!_isOpen)
            {
                throw new InvalidOperationException("The stream is closed");
            }
        }

        private void EnsureValueIsCorrect(long value)
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value),
                    "Value shouldn't be negative");
            }

            if (value > MemStreamMaxLength)
            {
                throw new ArgumentOutOfRangeException(nameof(value),
                    $"Value shouldn't be greater than {MemStreamMaxLength}");
            }
        }
    }
}
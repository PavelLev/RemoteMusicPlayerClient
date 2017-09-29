using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RemoteMusicPlayerClient.Utility.Segments;

namespace RemoteMusicPlayerClient.Utility
{
    public class RemoteFileReader: Stream
    {
        private const int MemStreamMaxLength = Int32.MaxValue;
        private int _position;
        private readonly int _length;
        private bool _isOpen = true;
        private readonly byte[] _buffer;
        private readonly SegmentCollection _localSegments;
        private readonly NetworkStream _networkStream;

        private readonly JsonTextWriter _jsonTextWriter;

        public RemoteFileReader(int length, NetworkStream networkStream)
        {
            _length = length;

            _localSegments = new SegmentCollection(length);

            _buffer = new byte[length];

            _networkStream = networkStream;
            _jsonTextWriter = new JsonTextWriter(new StreamWriter(networkStream));
        }

        public static async Task<RemoteFileReader> ByToken(string token)
        {
            var tcpClient = new TcpClient();

            await tcpClient.ConnectAsync("localhost", 54364);

            var networkStream = tcpClient.GetStream();

            networkStream.WriteAsync(token);

            var intBuffer = new byte[4];
            var result = await networkStream.ReadAsync(intBuffer, 0, 4);
            if (result != 4)
            {
                throw new NetworkInformationException();
            }
            var length = BitConverter.ToInt32(intBuffer, 0);

            return new RemoteFileReader(length, networkStream);
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

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer), "ArgumentNull_Buffer");
            }
            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(offset), "ArgumentOutOfRange_NeedNonNegNum");
            }
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count), "ArgumentOutOfRange_NeedNonNegNum");
            }
            if (offset + count > buffer.Length)
            {
                throw new ArgumentException("Argument_InvalidOffLen");
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

            var readTasks = absentSegments.Select(absentSegment =>
            {
                Serialization.Serializer.Serialize(_jsonTextWriter, absentSegment);
                _jsonTextWriter.Flush();

                return _networkStream.ReadAsync(_buffer, absentSegment.Begin, absentSegment.Count);
            }).ToArray();
            Task.WaitAll(readTasks);

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
            _isOpen = false;
            _networkStream.Close();
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
                    "Position cannot be set to a value less than zero");
            }

            if (value > MemStreamMaxLength)
            {
                throw new ArgumentOutOfRangeException(nameof(value),
                    $"Position cannot be set to a value greater than {MemStreamMaxLength}");
            }
        }
    }
}
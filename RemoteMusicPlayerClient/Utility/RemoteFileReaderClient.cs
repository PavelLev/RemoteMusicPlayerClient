using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;
using RemoteMusicPlayerClient.Utility.Segments;

namespace RemoteMusicPlayerClient.Utility
{
    public class RemoteFileReaderClient: Stream
    {
        private const int MemStreamMaxLength = Int32.MaxValue;
        private int _position = 0;
        private readonly int _length;
        private bool _isOpen = true;
        private readonly byte[] _buffer;
        private readonly SegmentCollection _localSegments;
        private readonly NetworkStream _networkStream;

        public RemoteFileReaderClient(int length, NetworkStream networkStream)
        {
            _length = length;
            _networkStream = networkStream;
            _buffer = new byte[length];
            _localSegments = new SegmentCollection(length);
        }

        public static async Task<RemoteFileReaderClient> ByToken(string token)
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

            return new RemoteFileReaderClient(length, networkStream);
        }
        

        public override void Flush()
        {
            throw new NotImplementedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            EnsureStreamIsOpen();

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
                    throw new ArgumentException("Argument_InvalidSeekOrigin");
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
            EnsureStreamIsOpen();


            var numberOfBytesToRead = _length - _position;
            if (numberOfBytesToRead > count)
            {
                numberOfBytesToRead = count;
            }
            if (numberOfBytesToRead <= 0)
            {
                return 0;
            }

            // TODO Network interaction goes here
            var absentSegments = _localSegments.Add(new Segment(_position, _position + numberOfBytesToRead - 1));

            foreach (var absentSegment in absentSegments)
            {
                _networkStream
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
                EnsureStreamIsOpen();
                return _length;
            }
        }

        public override long Position
        {
            get
            {
                EnsureStreamIsOpen();
                return _position;
            }
            set
            {
                EnsureStreamIsOpen();

                EnsureValueIsCorrect(value);

                _position = (int)value;
            }
        }

        private void EnsureStreamIsOpen()
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
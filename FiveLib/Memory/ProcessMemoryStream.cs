using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace FiveLib.Memory
{
    /// <summary>
    /// A stream which reads/writes another process's memory.
    /// </summary>
    public class ProcessMemoryStream : Stream
    {
        private const int BufferSize = 0x1000;
        private const ulong PositionMask = 0xFFF; // Mask to get the buffer offset from the position

        private ulong _position;

        private readonly byte[] _readBuffer; // null if buffering is disabled
        private ulong _bufferStart;
        private int _bufferLength;

        /// <summary>
        /// Constructs a <see cref="ProcessMemoryStream"/> that accesses the memory of a process.
        /// </summary>
        /// <param name="process">The process to access the memory of.</param>
        /// <param name="buffered"><c>true</c> if read buffering should be used.</param>
        public ProcessMemoryStream(Process process, bool buffered)
        {
            BaseProcess = process;
            _position = (ulong)process.MainModule.BaseAddress;
            if (buffered)
                _readBuffer = new byte[BufferSize];
        }

        /// <summary>
        /// Gets the process that the stream operates on.
        /// </summary>
        public Process BaseProcess { get; }

        public override bool CanRead => true;

        public override bool CanSeek => true;

        public override bool CanWrite => true;

        public override long Length => long.MaxValue;

        public override long Position
        {
            get { return (long)_position; }
            set { _position = (ulong)value; }
        }

        public override void Flush()
        {
            _bufferLength = 0; // Invalidate the buffer
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            switch (origin)
            {
                case SeekOrigin.Begin:
                    _position = (ulong)offset;
                    break;
                case SeekOrigin.Current:
                    _position += (ulong)offset;
                    break;
                case SeekOrigin.End:
                    _position = (ulong)Length + (ulong)offset;
                    break;
                default:
                    throw new ArgumentException($"Invalid seek origin {origin}");
            }
            return (long)_position;
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return _readBuffer != null ? BufferedRead(buffer, offset, count) : RawRead(buffer, offset, count);
        }

        private int BufferedRead(byte[] buffer, int offset, int count)
        {
            var bytesRead = 0;
            var bytesRemaining = count;
            while (bytesRemaining > 0 && FillBuffer())
            {
                var bufferPosition = ToBufferPosition(_position);
                var bytesAvailable = Math.Min(bytesRemaining, _bufferLength - bufferPosition);
                Buffer.BlockCopy(_readBuffer, bufferPosition, buffer, offset + bytesRead, bytesAvailable);
                _position += (ulong)bytesAvailable;
                bytesRead += bytesAvailable;
                bytesRemaining -= bytesAvailable;
            }
            return bytesRead;
        }

        private int RawRead(byte[] buffer, int offset, int count)
        {
            var bytesRead = ReadMemory(BaseProcess, _position, buffer, offset, count);
            _position += (ulong)bytesRead;
            return bytesRead;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            var bytesWritten = WriteMemory(BaseProcess, _position, buffer, offset, count);
            if (InsideBuffer())
            {
                // Update the read buffer to reflect the written bytes
                var bufferPosition = ToBufferPosition(_position);
                var bytesAvailable = Math.Min(bytesWritten, _bufferLength - bufferPosition);
                Buffer.BlockCopy(buffer, offset, _readBuffer, bufferPosition, bytesAvailable);
            }
            _position += (ulong)count;
        }

        private bool FillBuffer()
        {
            if (InsideBuffer())
                return true;

            // Read the page where the position is located
            var pageStart = _position & ~PositionMask;
            var bytesRead = ReadMemory(BaseProcess, pageStart, _readBuffer, 0, _readBuffer.Length);
            _bufferStart = pageStart;
            _bufferLength = bytesRead;
            return InsideBuffer();
        }

        private bool InsideBuffer()
        {
            return _readBuffer != null && _position >= _bufferStart && _position < _bufferStart + (ulong)_bufferLength;
        }

        private static int ToBufferPosition(ulong position)
        {
            return (int)(position & PositionMask);
        }

        private static unsafe int ReadMemory(Process process, ulong address, byte[] buffer, int offset, int count)
        {
            var bytesAvailable = Math.Min(count, buffer.Length - offset);
            UIntPtr bytesRead;
            fixed (byte* pBuffer = buffer)
            {
                if (!ReadProcessMemory(process.Handle, (IntPtr)address, pBuffer + offset, (UIntPtr)bytesAvailable, out bytesRead))
                    return 0;
            }
            return (int)bytesRead;
        }

        private static unsafe int WriteMemory(Process process, ulong address, byte[] buffer, int offset, int count)
        {
            var bytesAvailable = Math.Min(count, buffer.Length - offset);
            UIntPtr bytesWritten;
            fixed (byte* pBuffer = buffer)
            {
                if (!WriteProcessMemory(process.Handle, (IntPtr)address, pBuffer + offset, (UIntPtr)bytesAvailable, out bytesWritten))
                    return 0;
            }
            return (int)bytesWritten;
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern unsafe bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte* lpBuffer,
            UIntPtr nSize, out UIntPtr lpNumberOfBytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern unsafe bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte* lpBuffer,
            UIntPtr nSize, out UIntPtr lpNumberOfBytesWritten);
    }
}
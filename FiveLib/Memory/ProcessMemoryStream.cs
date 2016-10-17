using System;
using System.Collections.Generic;
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
        private const int PageSize = 0x1000;
        private const ulong PageIndexMask = ~0xFFFUL;
        private const ulong PageOffsetMask = 0xFFFUL;

        private ulong _position;

        private readonly LinkedList<CachedPage> _pages = new LinkedList<CachedPage>(); // Page at the front is most-recently used
        private readonly Dictionary<ulong, LinkedListNode<CachedPage>> _pagesByAddress = new Dictionary<ulong, LinkedListNode<CachedPage>>();
        private readonly int _cacheSize;

        /// <summary>
        /// Constructs a <see cref="ProcessMemoryStream"/> that accesses the memory of a process.
        /// </summary>
        /// <param name="process">The process to access the memory of.</param>
        /// <param name="cacheSize">The maximum number of pages to cache.</param>
        public ProcessMemoryStream(Process process, int cacheSize = 8)
        {
            BaseProcess = process;
            _position = (ulong)process.MainModule.BaseAddress;
            _cacheSize = cacheSize;
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
            _pages.Clear();
            _pagesByAddress.Clear();
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
            return _cacheSize > 0 ? CachedRead(buffer, offset, count) : RawRead(buffer, offset, count);
        }

        private int CachedRead(byte[] buffer, int offset, int count)
        {
            var bytesRead = 0;
            var bytesRemaining = count;
            byte[] page;
            while (bytesRemaining > 0 && (page = LoadCurrentPage()) != null)
            {
                var pageOffset = GetPageOffset(_position);
                var bytesAvailable = Math.Min(bytesRemaining, PageSize - pageOffset);
                Buffer.BlockCopy(page, pageOffset, buffer, offset + bytesRead, bytesAvailable);
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
            WriteMemory(BaseProcess, _position, buffer, offset, count);
            _position += (ulong)count;
            Flush(); // Just purge the cache for now instead of trying to update it...
        }

        private byte[] GetCurrentPage()
        {
            var pageAddress = GetPageAddress(_position);
            if (_pages.Count > 0 && _pages.First.Value.Address == pageAddress)
                return _pages.First.Value.Data;

            LinkedListNode<CachedPage> node;
            if (!_pagesByAddress.TryGetValue(pageAddress, out node))
                return null;

            // Bump the page to the front of the list because it's been used
            _pages.Remove(node);
            _pages.AddFirst(node);
            return node.Value.Data;
        }

        private byte[] LoadCurrentPage()
        {
            var data = GetCurrentPage();
            if (data != null)
                return data;
            var pageAddress = GetPageAddress(_position);
            data = _pages.Count < _cacheSize ? new byte[PageSize] : EvictPage();
            var bytesRead = ReadMemory(BaseProcess, pageAddress, data, 0, data.Length);
            if (bytesRead == 0)
                return null;
            var node = _pages.AddFirst(new CachedPage(pageAddress, data));
            _pagesByAddress[pageAddress] = node;
            return data;
        }

        private byte[] EvictPage()
        {
            var lastPage = _pages.Last.Value; // Least recently used
            _pages.RemoveLast();
            _pagesByAddress.Remove(lastPage.Address);
            return lastPage.Data;
        }

        private static ulong GetPageAddress(ulong position)
        {
            return position & PageIndexMask;
        }

        private static int GetPageOffset(ulong position)
        {
            return (int)(position & PageOffsetMask);
        }

        private class CachedPage
        {
            public CachedPage(ulong address, byte[] data)
            {
                Address = address;
                Data = data;
            }

            public ulong Address { get; }
            public byte[] Data { get; }
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
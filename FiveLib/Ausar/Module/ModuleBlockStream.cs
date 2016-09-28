using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FiveLib.Common;

namespace FiveLib.Ausar.Module
{
    public class ModuleBlockStream : Stream
    {
        private readonly ModuleBlockCompressor _blockCompressor;
        private readonly List<ModuleEntryBlock> _blocks; // Sorted by uncompressed offset for quick seeking
        private readonly long _entryOffset;
        private readonly long _length;

        private long _position;
        private ModuleEntryBlock _currentBlock; // Can be null
        private MemoryStream _currentBlockStream; // Can be null

        public ModuleBlockStream(ModuleBlockCompressor blockCompressor, ModuleEntry entry)
        {
            _blockCompressor = blockCompressor;
            _blocks = entry.Blocks.OrderBy(b => b.UncompressedOffset).ToList();
            _entryOffset = entry.CompressedOffset;
            _length = entry.TotalUncompressedSize;
        }
         
        protected override void Dispose(bool disposing)
        {
            if (disposing)
                CloseBlock();
        }

        public override void Flush()
        {
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            switch (origin)
            {
                case SeekOrigin.Begin:
                    Position = offset;
                    break;
                case SeekOrigin.Current:
                    Position += offset;
                    break;
                case SeekOrigin.End:
                    Position = Length + offset;
                    break;
                default:
                    throw new ArgumentException($"Invalid seek origin {origin}", nameof(origin));
            }
            return Position;
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException("Setting the length of the stream is not supported");
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            var totalBytesRead = 0;
            var bytesRemaining = count;
            while (bytesRemaining > 0 && LoadBlock())
            {
                _currentBlockStream.Position = Position - _currentBlock.UncompressedOffset;
                var bytesRead = _currentBlockStream.Read(buffer, offset + totalBytesRead, bytesRemaining);
                totalBytesRead += bytesRead;
                Position += bytesRead;
                bytesRemaining -= bytesRead;
            }
            return totalBytesRead;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException("Writing is not supported");
        }

        public override bool CanRead => true;
        public override bool CanSeek => true;
        public override bool CanWrite => false;
        public override long Length => _length;

        public override long Position
        {
            get { return _position; }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), "Stream position must be non-negative");
                _position = value;
            }
        }

        private bool LoadBlock()
        {
            if (_currentBlock != null && IsInBlock(Position, _currentBlock))
                return true;
            CloseBlock();
            var index = FindBlock(_blocks, Position);
            if (index < 0)
                return false;
            var block = _blocks[index];
            var blockStream = new MemoryStream((int)block.UncompressedSize);
            _blockCompressor.ReadBlock(_entryOffset, block, blockStream);
            _currentBlock = block;
            _currentBlockStream = blockStream;
            return true;
        }

        private static int FindBlock(IList<ModuleEntryBlock> blocks, long position)
        {
            var index = BinarySearch.Search(blocks, position, b => b.UncompressedOffset);
            if (index < 0)
                index = ~index - 1; // If no exact match is found, take the closest block before the position
            if (index < 0 || index >= blocks.Count || !IsInBlock(position, blocks[index]))
                return -1;
            return index;
        }

        private void CloseBlock()
        {
            _currentBlockStream?.Dispose();
            _currentBlockStream = null;
            _currentBlock = null;
        }

        private static bool IsInBlock(long position, ModuleEntryBlock block)
        {
            return position >= block.UncompressedOffset && position < block.UncompressedOffset + block.UncompressedSize;
        }
    }
}

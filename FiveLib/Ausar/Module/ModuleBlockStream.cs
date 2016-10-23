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
        private readonly List<ModuleDataBlock> _blocks; // Sorted by uncompressed offset for quick seeking
        private readonly long _entryOffset;
        private readonly long _sectionOffset; // Offset of the section relative to the start of the entry
        private readonly long _sectionLength;

        private long _sectionPosition; // Position relative to the start of the section
        private long _entryPosition; // Position relative to the start of the entry
        private ModuleDataBlock _currentBlock; // Can be null
        private MemoryStream _currentBlockStream; // Can be null

        public ModuleBlockStream(ModuleBlockCompressor blockCompressor, ModuleEntry entry, ModuleEntrySection section)
        {
            _blockCompressor = blockCompressor;
            _entryOffset = entry.DataOffset;
            _blocks = GetBlocks(entry, section);
            if (_blocks.Count > 0)
            {
                _sectionOffset = _blocks[0].UncompressedOffset;
                _entryPosition = _sectionOffset;
                var lastBlock = _blocks[_blocks.Count - 1];
                _sectionLength = lastBlock.UncompressedOffset + lastBlock.UncompressedSize - _sectionOffset;
            }
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
                _currentBlockStream.Position = _entryPosition - _currentBlock.UncompressedOffset;
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
        public override long Length => _sectionLength;

        public override long Position
        {
            get { return _sectionPosition; }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), "Stream position must be non-negative");
                _sectionPosition = value;
                _entryPosition = _sectionOffset + _sectionPosition;
            }
        }

        private bool LoadBlock()
        {
            if (_blocks.Count == 0)
                return false;
            if (_currentBlock != null && IsInBlock(_entryPosition, _currentBlock))
                return true;
            CloseBlock();
            var index = FindBlock(_blocks, _entryPosition);
            if (index < 0)
                return false;
            var block = _blocks[index];
            var blockStream = new MemoryStream((int)block.UncompressedSize);
            _blockCompressor.ReadBlock(_entryOffset, block, blockStream);
            _currentBlock = block;
            _currentBlockStream = blockStream;
            return true;
        }

        private static List<ModuleDataBlock> GetBlocks(ModuleEntry entry, ModuleEntrySection section)
        {
            if (entry.IsRawFile && section != ModuleEntrySection.All)
                throw new ArgumentException($"Raw module entries cannot be accessed per-section");
            int start, count;
            switch (section)
            {
                case ModuleEntrySection.Header:
                    start = 0;
                    count = entry.HeaderBlockCount;
                    break;
                case ModuleEntrySection.TagData:
                    start = entry.HeaderBlockCount;
                    count = entry.TagDataBlockCount;
                    break;
                case ModuleEntrySection.ResourceData:
                    start = entry.HeaderBlockCount + entry.TagDataBlockCount;
                    count = entry.ResourceBlockCount;
                    break;
                case ModuleEntrySection.All:
                    start = 0;
                    count = entry.Blocks.Count;
                    break;
                default:
                    throw new ArgumentException($"Unsupported module entry section {section}", nameof(section));
            }
            return entry.Blocks
                .Skip(start)
                .Take(count)
                .OrderBy(b => b.UncompressedOffset)
                .ToList();
        } 

        private static int FindBlock(IList<ModuleDataBlock> blocks, long position)
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

        private static bool IsInBlock(long position, ModuleDataBlock block)
        {
            return position >= block.UncompressedOffset && position < block.UncompressedOffset + block.UncompressedSize;
        }
    }
}

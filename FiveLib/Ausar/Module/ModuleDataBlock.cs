using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FiveLib.Ausar.Module.Structures;

namespace FiveLib.Ausar.Module
{
    public class ModuleDataBlock
    {
        public ulong Checksum { get; }

        public uint CompressedOffset { get; }

        public uint CompressedSize { get; }

        public uint UncompressedOffset { get; }

        public uint UncompressedSize { get; }

        public bool IsCompressed { get; }

        internal ModuleDataBlock(ModuleDataBlockStruct data)
        {
            Checksum = data.Checksum;
            CompressedOffset = data.CompressedOffset;
            CompressedSize = data.CompressedSize;
            UncompressedOffset = data.UncompressedOffset;
            UncompressedSize = data.UncompressedSize;
            IsCompressed = data.IsCompressed;
        }
    }
}

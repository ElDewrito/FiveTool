using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FiveLib.Ausar.Module.Structures;

namespace FiveLib.Ausar.Module
{
    public class ModuleEntryBlock
    {
        public long Unknown0 { get; }

        public uint CompressedOffset { get; }

        public uint CompressedSize { get; }

        public uint UncompressedOffset { get; }

        public uint UncompressedSize { get; }

        public int Unknown18 { get; }

        public int Unknown1C { get; }

        internal ModuleEntryBlock(ModuleCompressedBlockStruct data)
        {
            Unknown0 = data.Unknown0;
            CompressedOffset = data.CompressedOffset;
            CompressedSize = data.CompressedSize;
            UncompressedOffset = data.UncompressedOffset;
            UncompressedSize = data.UncompressedSize;
            Unknown18 = data.Unknown18;
            Unknown1C = data.Unknown1C;
        }
    }
}

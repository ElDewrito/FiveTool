using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FiveLib.Ausar.Module.Structures;
using FiveLib.Common;

namespace FiveLib.Ausar.Module
{
    /// <summary>
    /// Contains information about a file embedded inside a module.
    /// </summary>
    [DebuggerDisplay("ModuleEntry {Name}")]
    public class ModuleEntry
    {
        public string Name { get; }

        public int ParentIndex { get; }

        public int Unknown8 { get; }

        public int UnknownC { get; }

        public IList<ModuleEntryBlock> Blocks { get; }

        public ulong CompressedOffset { get; }

        public uint TotalCompressedSize { get; }

        public uint TotalUncompressedSize { get; }

        public byte Unknown28 { get; }

        public byte Unknown29 { get; }

        public byte Unknown2A { get; }

        public byte Unknown2B { get; }

        public int GlobalTagId { get; }

        public long SourceTagId { get; }

        public long Unknown38 { get; }

        public MagicNumber GroupTag { get; }

        public uint UncompressedHeaderSize { get; }

        public uint UncompressedTagDataSize { get; }

        public uint UncompressedResourceDataSize { get; }

        public short Unknown50 { get; }

        public short Unknown52 { get; }

        public int Unknown54 { get; }

        internal ModuleEntry(ModuleEntryStruct data, ModuleStruct module)
        {
            Name = module.StringTable.GetStringAtOffset((int)data.NameOffset);
            ParentIndex = data.ParentFileIndex;
            Unknown8 = data.Unknown8;
            UnknownC = data.UnknownC;
            Blocks = GetBlockList(data, module).AsReadOnly();
            CompressedOffset = data.CompressedOffset;
            TotalCompressedSize = data.TotalCompressedSize;
            TotalUncompressedSize = data.TotalUncompressedSize;
            Unknown28 = data.Unknown28;
            Unknown29 = data.Unknown29;
            Unknown2A = data.Unknown2A;
            Unknown2B = data.Unknown2B;
            GlobalTagId = data.GlobalTagId;
            SourceTagId = data.SourceTagId;
            Unknown38 = data.Unknown38;
            GroupTag = data.GroupTag;
            UncompressedHeaderSize = data.UncompressedHeaderSize;
            UncompressedTagDataSize = data.UncompressedTagDataSize;
            UncompressedResourceDataSize = data.UncompressedResourceDataSize;
            Unknown50 = data.Unknown50;
            Unknown52 = data.Unknown52;
            Unknown54 = data.Unknown54;
        }

        private static List<ModuleEntryBlock> GetBlockList(ModuleEntryStruct data, ModuleStruct module)
        {
            var blocks = new List<ModuleEntryBlock>(data.BlockCount);
            for (var i = 0; i < data.BlockCount; i++)
            {
                var block = module.CompressedBlocks[data.FirstBlockIndex + i];
                blocks.Add(new ModuleEntryBlock(block));
            }
            return blocks;
        } 
    }
}

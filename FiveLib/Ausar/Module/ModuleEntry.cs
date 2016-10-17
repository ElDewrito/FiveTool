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
        private readonly List<ModuleEntry> _resources = new List<ModuleEntry>();

        public int Index { get; }
         
        public string Name { get; }

        public int ParentIndex { get; }

        public IList<ModuleEntry> Resources { get; } 

        public IList<ModuleDataBlock> Blocks { get; }

        public long DataOffset { get; }

        public uint TotalCompressedSize { get; }

        public uint TotalUncompressedSize { get; }

        public byte HeaderAlignment { get; }

        public byte TagDataAlignment { get; }

        public byte ResourceDataAlignment { get; }

        public bool IsRawFile { get; }

        public uint GlobalId { get; }

        public ulong AssetId { get; }

        public ulong AssetChecksum { get; }

        public MagicNumber GroupTag { get; }

        public uint UncompressedHeaderSize { get; }

        public uint UncompressedTagDataSize { get; }

        public uint UncompressedResourceDataSize { get; }

        public short HeaderBlockCount { get; }

        public short TagDataBlockCount { get; }

        public int ResourceBlockCount { get; }

        internal ModuleEntry(int index, ModuleEntryStruct data, ModuleStruct module)
        {
            Index = index;
            Name = module.StringTable.GetStringAtOffset((int)data.NameOffset);
            ParentIndex = data.ParentFileIndex;
            Resources = _resources.AsReadOnly();
            Blocks = GetBlockList(data, module).AsReadOnly();
            DataOffset = data.DataOffset;
            TotalCompressedSize = data.TotalCompressedSize;
            TotalUncompressedSize = data.TotalUncompressedSize;
            HeaderAlignment = data.HeaderAlignment;
            TagDataAlignment = data.TagDataAlignment;
            ResourceDataAlignment = data.ResourceDataAlignment;
            IsRawFile = (data.Flags & ModuleEntryFlags.RawFile) != 0;
            GlobalId = data.GlobalId;
            AssetId = data.AssetId;
            AssetChecksum = data.AssetChecksum;
            GroupTag = data.GroupTag;
            UncompressedHeaderSize = data.UncompressedHeaderSize;
            UncompressedTagDataSize = data.UncompressedTagDataSize;
            UncompressedResourceDataSize = data.UncompressedResourceDataSize;
            HeaderBlockCount = data.HeaderBlockCount;
            TagDataBlockCount = data.TagDataBlockCount;
            ResourceBlockCount = data.ResourceBlockCount;
        }

        internal void BuildResourceList(ModuleEntryStruct data, List<ModuleEntry> resources)
        {
            for (var i = 0; i < data.ResourceCount; i++)
                _resources.Add(resources[data.FirstResourceIndex + i]);
        }

        private static List<ModuleDataBlock> GetBlockList(ModuleEntryStruct data, ModuleStruct module)
        {
            var blocks = new List<ModuleDataBlock>(data.BlockCount);
            if (data.TotalCompressedSize == 0)
                return blocks;
            if ((data.Flags & ModuleEntryFlags.HasBlocks) != 0)
            {
                for (var i = 0; i < data.BlockCount; i++)
                {
                    var block = module.CompressedBlocks[data.FirstBlockIndex + i];
                    blocks.Add(new ModuleDataBlock(block));
                }
            }
            else
            {
                // If an entry has 0 blocks, then make one
                var blockStruct = new ModuleDataBlockStruct
                {
                    Checksum = data.AssetChecksum,
                    CompressedOffset = 0,
                    CompressedSize = data.TotalCompressedSize,
                    IsCompressed = (data.Flags & ModuleEntryFlags.Compressed) != 0,
                    UncompressedOffset = 0,
                    UncompressedSize = data.TotalUncompressedSize,
                };
                blocks.Add(new ModuleDataBlock(blockStruct));
            }
            return blocks;
        } 
    }
}

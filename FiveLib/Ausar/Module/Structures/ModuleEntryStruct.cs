using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FiveLib.Common;

namespace FiveLib.Ausar.Module.Structures
{
    [Flags]
    internal enum ModuleEntryFlags : byte
    {
        Compressed = 1 << 0,
        HasBlocks = 1 << 1,
        RawFile = 1 << 2,
    }

    internal class ModuleEntryStruct
    {
        public uint NameOffset { get; set; }

        public int ParentFileIndex { get; set; } = -1;

        public int ResourceCount { get; set; }

        public int FirstResourceIndex { get; set; }

        public int BlockCount { get; set; }

        public int FirstBlockIndex { get; set; }

        public long DataOffset { get; set; }

        public uint TotalCompressedSize { get; set; }

        public uint TotalUncompressedSize { get; set; }

        public byte HeaderAlignment { get; set; }

        public byte TagDataAlignment { get; set; }

        public byte ResourceDataAlignment { get; set; }

        public ModuleEntryFlags Flags { get; set; }

        public uint GlobalId { get; set; }

        public ulong AssetId { get; set; }

        public ulong AssetChecksum { get; set; }

        public MagicNumber GroupTag { get; set; }

        public uint UncompressedHeaderSize { get; set; }

        public uint UncompressedTagDataSize { get; set; }

        public uint UncompressedResourceDataSize { get; set; }

        public short HeaderBlockCount { get; set; }

        public short TagDataBlockCount { get; set; }

        public int ResourceBlockCount { get; set; }

        public void Read(BinaryReader reader)
        {
            NameOffset = reader.ReadUInt32();
            ParentFileIndex = reader.ReadInt32();
            ResourceCount = reader.ReadInt32();
            FirstResourceIndex = reader.ReadInt32();
            BlockCount = reader.ReadInt32();
            FirstBlockIndex = reader.ReadInt32();
            DataOffset = reader.ReadInt64();
            TotalCompressedSize = reader.ReadUInt32();
            TotalUncompressedSize = reader.ReadUInt32();
            HeaderAlignment = reader.ReadByte();
            TagDataAlignment = reader.ReadByte();
            ResourceDataAlignment = reader.ReadByte();
            Flags = (ModuleEntryFlags)reader.ReadByte();
            GlobalId = reader.ReadUInt32();
            AssetId = reader.ReadUInt64();
            AssetChecksum = reader.ReadUInt64();
            GroupTag = new MagicNumber(reader.ReadInt32());
            UncompressedHeaderSize = reader.ReadUInt32();
            UncompressedTagDataSize = reader.ReadUInt32();
            UncompressedResourceDataSize = reader.ReadUInt32();
            HeaderBlockCount = reader.ReadInt16();
            TagDataBlockCount = reader.ReadInt16();
            ResourceBlockCount = reader.ReadInt32();
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write(NameOffset);
            writer.Write(ParentFileIndex);
            writer.Write(ResourceCount);
            writer.Write(FirstResourceIndex);
            writer.Write(BlockCount);
            writer.Write(FirstBlockIndex);
            writer.Write(DataOffset);
            writer.Write(TotalCompressedSize);
            writer.Write(TotalUncompressedSize);
            writer.Write(HeaderAlignment);
            writer.Write(TagDataAlignment);
            writer.Write(ResourceDataAlignment);
            writer.Write((byte)Flags);
            writer.Write(GlobalId);
            writer.Write(AssetId);
            writer.Write(AssetChecksum);
            writer.Write(GroupTag.Value);
            writer.Write(UncompressedHeaderSize);
            writer.Write(UncompressedTagDataSize);
            writer.Write(UncompressedResourceDataSize);
            writer.Write(HeaderBlockCount);
            writer.Write(TagDataBlockCount);
            writer.Write(ResourceBlockCount);
        }
    }
}

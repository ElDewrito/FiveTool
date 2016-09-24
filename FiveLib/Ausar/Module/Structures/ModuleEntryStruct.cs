using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FiveLib.Common;

namespace FiveLib.Ausar.Module.Structures
{
    internal class ModuleEntryStruct
    {
        public uint NameOffset { get; set; }

        public int ParentFileIndex { get; set; }

        public int Unknown8 { get; set; }

        public int UnknownC { get; set; }

        public int BlockCount { get; set; }

        public int FirstBlockIndex { get; set; }

        public ulong CompressedOffset { get; set; }

        public uint TotalCompressedSize { get; set; }

        public uint TotalUncompressedSize { get; set; }

        public byte Unknown28 { get; set; }

        public byte Unknown29 { get; set; }

        public byte Unknown2A { get; set; }

        public byte Unknown2B { get; set; }

        public int GlobalTagId { get; set; }

        public long SourceTagId { get; set; }

        public long Unknown38 { get; set; }

        public MagicNumber GroupTag { get; set; }

        public uint UncompressedHeaderSize { get; set; }

        public uint UncompressedTagDataSize { get; set; }

        public uint UncompressedResourceDataSize { get; set; }

        public short Unknown50 { get; set; }

        public short Unknown52 { get; set; }

        public int Unknown54 { get; set; }

        public void Read(BinaryReader reader)
        {
            NameOffset = reader.ReadUInt32();
            ParentFileIndex = reader.ReadInt32();
            Unknown8 = reader.ReadInt32();
            UnknownC = reader.ReadInt32();
            BlockCount = reader.ReadInt32();
            FirstBlockIndex = reader.ReadInt32();
            CompressedOffset = reader.ReadUInt64();
            TotalCompressedSize = reader.ReadUInt32();
            TotalUncompressedSize = reader.ReadUInt32();
            Unknown28 = reader.ReadByte();
            Unknown29 = reader.ReadByte();
            Unknown2A = reader.ReadByte();
            Unknown2B = reader.ReadByte();
            GlobalTagId = reader.ReadInt32();
            SourceTagId = reader.ReadInt64();
            Unknown38 = reader.ReadInt64();
            GroupTag = new MagicNumber(reader.ReadInt32());
            UncompressedHeaderSize = reader.ReadUInt32();
            UncompressedTagDataSize = reader.ReadUInt32();
            UncompressedResourceDataSize = reader.ReadUInt32();
            Unknown50 = reader.ReadInt16();
            Unknown52 = reader.ReadInt16();
            Unknown54 = reader.ReadInt32();
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write(NameOffset);
            writer.Write(ParentFileIndex);
            writer.Write(Unknown8);
            writer.Write(UnknownC);
            writer.Write(BlockCount);
            writer.Write(FirstBlockIndex);
            writer.Write(CompressedOffset);
            writer.Write(TotalCompressedSize);
            writer.Write(TotalUncompressedSize);
            writer.Write(Unknown28);
            writer.Write(Unknown29);
            writer.Write(Unknown2A);
            writer.Write(Unknown2B);
            writer.Write(GlobalTagId);
            writer.Write(SourceTagId);
            writer.Write(Unknown38);
            writer.Write(GroupTag.Value);
            writer.Write(UncompressedHeaderSize);
            writer.Write(UncompressedTagDataSize);
            writer.Write(UncompressedResourceDataSize);
            writer.Write(Unknown50);
            writer.Write(Unknown52);
            writer.Write(Unknown54);
        }
    }
}

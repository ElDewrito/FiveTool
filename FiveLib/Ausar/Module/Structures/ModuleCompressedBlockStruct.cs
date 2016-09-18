using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveLib.Ausar.Module.Structures
{
    internal class ModuleCompressedBlockStruct
    {
        public long Unknown0 { get; set; }

        public uint CompressedOffset { get; set; }

        public uint CompressedSize { get; set; }

        public uint UncompressedOffset { get; set; }

        public uint UncompressedSize { get; set; }

        public int Unknown18 { get; set; }

        public int Unknown1C { get; set; } 

        public void Read(BinaryReader reader)
        {
            Unknown0 = reader.ReadInt64();
            CompressedOffset = reader.ReadUInt32();
            CompressedSize = reader.ReadUInt32();
            UncompressedOffset = reader.ReadUInt32();
            UncompressedSize = reader.ReadUInt32();
            Unknown18 = reader.ReadInt32();
            Unknown1C = reader.ReadInt32();
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write(Unknown0);
            writer.Write(CompressedOffset);
            writer.Write(CompressedSize);
            writer.Write(UncompressedOffset);
            writer.Write(UncompressedSize);
            writer.Write(Unknown18);
            writer.Write(Unknown1C);
        }
    }
}

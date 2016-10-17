using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveLib.Ausar.Module.Structures
{
    internal class ModuleDataBlockStruct
    {
        // MurmurHash3_128 of the uncompressed block
        public ulong Checksum { get; set; }

        public uint CompressedOffset { get; set; }

        public uint CompressedSize { get; set; }

        public uint UncompressedOffset { get; set; }

        public uint UncompressedSize { get; set; }

        public bool IsCompressed { get; set; }

        public void Read(BinaryReader reader)
        {
            Checksum = reader.ReadUInt64();
            CompressedOffset = reader.ReadUInt32();
            CompressedSize = reader.ReadUInt32();
            UncompressedOffset = reader.ReadUInt32();
            UncompressedSize = reader.ReadUInt32();
            IsCompressed = reader.ReadInt32() != 0;
            reader.BaseStream.Position += 4; // Padding
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write(Checksum);
            writer.Write(CompressedOffset);
            writer.Write(CompressedSize);
            writer.Write(UncompressedOffset);
            writer.Write(UncompressedSize);
            writer.Write(IsCompressed ? 1 : 0);
            writer.Write(0); // Padding
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FiveLib.Common;

namespace FiveLib.Ausar.Module.Structures
{
    internal class ModuleFileHeaderStruct
    {
        public static readonly MagicNumber ExpectedMagic = new MagicNumber("dhom");
        public const int ExpectedVersion = 27;

        public MagicNumber Magic { get; set; } = ExpectedMagic;

        public int Version { get; set; } = ExpectedVersion;

        public int Unknown8 { get; set; }

        public int UnknownC { get; set; }

        public int FileCount { get; set; }

        public int Unknown14 { get; set; }

        public int Unknown18 { get; set; }

        public uint StringTableSize { get; set; }

        public int ResourceCount { get; set; }

        public int CompressedBlockCount { get; set; }

        public ulong Unknown28 { get; set; }

        public ulong HeaderChecksum { get; set; }

        public void Read(BinaryReader reader)
        {
            Magic = new MagicNumber(reader.ReadInt32());
            Version = reader.ReadInt32();
            Unknown8 = reader.ReadInt32();
            UnknownC = reader.ReadInt32();
            FileCount = reader.ReadInt32();
            Unknown14 = reader.ReadInt32();
            Unknown18 = reader.ReadInt32();
            StringTableSize = reader.ReadUInt32();
            ResourceCount = reader.ReadInt32();
            CompressedBlockCount = reader.ReadInt32();
            Unknown28 = reader.ReadUInt64();
            HeaderChecksum = reader.ReadUInt64();
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write(Magic.Value);
            writer.Write(Version);
            writer.Write(Unknown8);
            writer.Write(UnknownC);
            writer.Write(FileCount);
            writer.Write(Unknown14);
            writer.Write(Unknown18);
            writer.Write(StringTableSize);
            writer.Write(ResourceCount);
            writer.Write(CompressedBlockCount);
            writer.Write(Unknown28);
            writer.Write(HeaderChecksum);
        }
    }
}

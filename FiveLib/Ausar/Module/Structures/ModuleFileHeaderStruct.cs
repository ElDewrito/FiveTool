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

        public byte[] Unknown28 { get; } = new byte[0x10];

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
            reader.Read(Unknown28, 0, Unknown28.Length);
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
        }
    }
}

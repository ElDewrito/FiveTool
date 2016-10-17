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

        public ulong Id { get; set; }

        public int FileCount { get; set; }

        public int LoadedTagCount { get; set; }

        public int FirstResourceIndex { get; set; }

        public uint StringTableSize { get; set; }

        public int ResourceCount { get; set; }

        public int DataBlockCount { get; set; }

        public ulong BuildVersionId { get; set; }

        public ulong HeaderChecksum { get; set; }

        public void Read(BinaryReader reader)
        {
            Magic = new MagicNumber(reader.ReadInt32());
            Version = reader.ReadInt32();
            Id = reader.ReadUInt64();
            FileCount = reader.ReadInt32();
            LoadedTagCount = reader.ReadInt32();
            FirstResourceIndex = reader.ReadInt32();
            StringTableSize = reader.ReadUInt32();
            ResourceCount = reader.ReadInt32();
            DataBlockCount = reader.ReadInt32();
            BuildVersionId = reader.ReadUInt64();
            HeaderChecksum = reader.ReadUInt64();
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write(Magic.Value);
            writer.Write(Version);
            writer.Write(Id);
            writer.Write(FileCount);
            writer.Write(LoadedTagCount);
            writer.Write(FirstResourceIndex);
            writer.Write(StringTableSize);
            writer.Write(ResourceCount);
            writer.Write(DataBlockCount);
            writer.Write(BuildVersionId);
            writer.Write(HeaderChecksum);
        }
    }
}

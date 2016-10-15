using System.IO;
using FiveLib.Ausar.Memory.Stl;
using FiveLib.Common;

namespace FiveLib.Ausar.Memory.Tags
{
    public class MemoryTagInfo : IBinaryReadable, IBinaryWritable, IBinaryStruct
    {
        public ulong AssetId { get; set; }

        public ulong AssetChecksum { get; set; }

        public MagicNumber GroupTag { get; set; }

        public ulong Unknown20 { get; set; }

        public long Unknown28 { get; set; }

        public long Unknown30 { get; set; }

        public long Unknown38 { get; set; }

        public long Unknown40 { get; set; }

        public uint LocalHandle { get; set; }

        public void Read(BinaryReader reader)
        {
            AssetId = reader.ReadUInt64();
            AssetChecksum = reader.ReadUInt64();
            GroupTag = new MagicNumber(reader.ReadInt32());
            reader.BaseStream.Position += 4;
            Unknown20 = reader.ReadUInt64();
            Unknown28 = reader.ReadInt64();
            Unknown30 = reader.ReadInt64();
            Unknown38 = reader.ReadInt64();
            Unknown40 = reader.ReadInt64();
            LocalHandle = reader.ReadUInt32();
            reader.BaseStream.Position += 4;
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write(AssetId);
            writer.Write(AssetChecksum);
            writer.Write(GroupTag.Value);
            writer.BaseStream.Position += 4;
            writer.Write(Unknown20);
            writer.Write(Unknown28);
            writer.Write(Unknown30);
            writer.Write(Unknown38);
            writer.Write(Unknown40);
            writer.Write(LocalHandle);
            writer.BaseStream.Position += 4;
        }

        public ulong GetStructSize() => 0x48;

        public ulong GetStructAlignment() => 0x8;
    }
}

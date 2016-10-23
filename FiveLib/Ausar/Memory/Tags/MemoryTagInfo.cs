using System.IO;
using FiveLib.Ausar.Memory.Stl;
using FiveLib.Common;
using FiveLib.IO;

namespace FiveLib.Ausar.Memory.Tags
{
    public class MemoryTagInfo : IBinarySerializable, IFixedSize
    {
        public ulong AssetId;
        public ulong AssetChecksum;
        public MagicNumber GroupTag;
        public ulong Unknown20;
        public long Unknown28;
        public long Unknown30;
        public long Unknown38;
        public long Unknown40;
        public uint LocalHandle;

        public void Serialize(BinarySerializer s)
        {
            s.Value(ref AssetId);
            s.Value(ref AssetChecksum);
            s.Value(ref GroupTag);
            s.Padding(4);
            s.Value(ref Unknown20);
            s.Value(ref Unknown28);
            s.Value(ref Unknown30);
            s.Value(ref Unknown38);
            s.Value(ref Unknown40);
            s.Value(ref LocalHandle);
            s.Padding(4);
        }

        public ulong GetStructSize() => 0x48;

        public ulong GetStructAlignment() => 0x8;
    }
}

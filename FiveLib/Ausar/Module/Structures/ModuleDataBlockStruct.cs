using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FiveLib.IO;

namespace FiveLib.Ausar.Module.Structures
{
    internal class ModuleDataBlockStruct : IBinarySerializable
    {
        public ulong Checksum; // MurmurHash3_128 of the uncompressed block
        public uint CompressedOffset;
        public uint CompressedSize;
        public uint UncompressedOffset;
        public uint UncompressedSize;
        public int Compression;

        public void Serialize(BinarySerializer s)
        {
            s.Value(ref Checksum);
            s.Value(ref CompressedOffset);
            s.Value(ref CompressedSize);
            s.Value(ref UncompressedOffset);
            s.Value(ref UncompressedSize);
            s.Value(ref Compression);
            s.Padding(4);
        }
    }
}

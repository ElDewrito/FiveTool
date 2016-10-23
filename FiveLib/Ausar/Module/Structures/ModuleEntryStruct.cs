using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FiveLib.Common;
using FiveLib.IO;

namespace FiveLib.Ausar.Module.Structures
{
    [Flags]
    internal enum ModuleEntryFlags : byte
    {
        Compressed = 1 << 0,
        HasBlocks = 1 << 1,
        RawFile = 1 << 2,
    }

    internal class ModuleEntryStruct : IBinarySerializable
    {
        public uint NameOffset;
        public int ParentFileIndex = -1;
        public int ResourceCount;
        public int FirstResourceIndex;
        public int BlockCount;
        public int FirstBlockIndex;
        public long DataOffset;
        public uint TotalCompressedSize;
        public uint TotalUncompressedSize;
        public byte HeaderAlignment;
        public byte TagDataAlignment;
        public byte ResourceDataAlignment;
        public ModuleEntryFlags Flags;
        public uint GlobalId;
        public ulong AssetId;
        public ulong AssetChecksum;
        public MagicNumber GroupTag;
        public uint UncompressedHeaderSize;
        public uint UncompressedTagDataSize;
        public uint UncompressedResourceDataSize;
        public short HeaderBlockCount;
        public short TagDataBlockCount;
        public int ResourceBlockCount;

        public void Serialize(BinarySerializer s)
        {
            s.Value(ref NameOffset);
            s.Value(ref ParentFileIndex);
            s.Value(ref ResourceCount);
            s.Value(ref FirstResourceIndex);
            s.Value(ref BlockCount);
            s.Value(ref FirstBlockIndex);
            s.Value(ref DataOffset);
            s.Value(ref TotalCompressedSize);
            s.Value(ref TotalUncompressedSize);
            s.Value(ref HeaderAlignment);
            s.Value(ref TagDataAlignment);
            s.Value(ref ResourceDataAlignment);
            s.Enum(ref Flags);
            s.Value(ref GlobalId);
            s.Value(ref AssetId);
            s.Value(ref AssetChecksum);
            s.Value(ref GroupTag);
            s.Value(ref UncompressedHeaderSize);
            s.Value(ref UncompressedTagDataSize);
            s.Value(ref UncompressedResourceDataSize);
            s.Value(ref HeaderBlockCount);
            s.Value(ref TagDataBlockCount);
            s.Value(ref ResourceBlockCount);
        }
    }
}

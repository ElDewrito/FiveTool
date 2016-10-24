using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FiveLib.Common;
using FiveLib.IO;

namespace FiveLib.Ausar.Cache.Structures
{
    internal class CacheHeaderStruct : IBinarySerializable
    {
        public static readonly MagicNumber ExpectedMagic = new MagicNumber("hscu"); // Universal Cache Storage Header?
        public const int ExpectedVersion = 27;

        public MagicNumber Magic = ExpectedMagic;
        public int Version = ExpectedVersion;
        public ulong Unknown8;
        public ulong AssetChecksum;
        public uint Unknown18;
        public int DependencyCount;
        public int DataBlockCount;
        public int TagStructCount;
        public int DataReferenceCount;
        public int TagReferenceCount;
        public int StringIdCount;
        public uint StringTableSize;
        public uint ZoneSetInfoSize;
        public uint HeaderSize;
        public uint TagDataSize;
        public uint ResourceDataSize;
        public byte HeaderAlignment;
        public byte TagDataAlignment;
        public byte ResourceDataAlignment;
        public byte Unknown4B;
        public int Unknown4C;
        public TagDependencyStruct[] Dependencies = new TagDependencyStruct[0];
        public DataBlockDefinitionStruct[] DataBlocks = new DataBlockDefinitionStruct[0];
        public TagStructDefinitionStruct[] TagStructs = new TagStructDefinitionStruct[0];
        public DataReferenceDefinitionStruct[] DataReferences = new DataReferenceDefinitionStruct[0];
        public TagReferenceDefinitionStruct[] TagReferences = new TagReferenceDefinitionStruct[0];
        public StringIdDefinitionStruct[] StringIds = new StringIdDefinitionStruct[0];
        public StringBlob StringTable = new StringBlob(Encoding.UTF8);
        public byte[] ZoneSetInfo = new byte[0];

        public void Serialize(BinarySerializer s)
        {
            s.Expect(ss => ss.Value(ref Magic), nameof(Magic), ExpectedMagic);
            s.Expect(ss => ss.Value(ref Version), nameof(Version), ExpectedVersion);
            s.Value(ref Unknown8);
            s.Value(ref AssetChecksum);
            s.Value(ref Unknown18);
            s.Value(ref DependencyCount);
            s.Value(ref DataBlockCount);
            s.Value(ref TagStructCount);
            s.Value(ref DataReferenceCount);
            s.Value(ref TagReferenceCount);
            s.Value(ref StringIdCount);
            s.Value(ref StringTableSize);
            s.Value(ref ZoneSetInfoSize);
            s.Value(ref HeaderSize);
            s.Value(ref TagDataSize);
            s.Value(ref ResourceDataSize);
            s.Value(ref HeaderAlignment);
            s.Value(ref TagDataAlignment);
            s.Value(ref ResourceDataAlignment);
            s.Value(ref Unknown4B);
            s.Value(ref Unknown4C);
            s.Array(ref Dependencies, DependencyCount);
            s.Array(ref DataBlocks, DataBlockCount);
            s.Array(ref TagStructs, TagStructCount);
            s.Array(ref DataReferences, DataReferenceCount);
            s.Array(ref TagReferences, TagReferenceCount);
            s.Array(ref StringIds, StringIdCount);
            s.StringTable(ref StringTable, Encoding.UTF8, (int)StringTableSize);
            s.Array(ref ZoneSetInfo, (int)ZoneSetInfoSize);
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FiveLib.Ausar.Cache.Structures;
using FiveLib.IO;

namespace FiveLib.Ausar.Cache
{
    public class CacheHeader
    {
        private readonly List<TagDependency> _dependencies;
        private readonly List<DataBlockDefinition> _dataBlocks;
        private readonly List<TagStructDefinition> _tagStructs;
        private readonly List<DataReferenceDefinition> _dataReferences;
        private readonly List<TagReferenceDefinition> _tagReferences;
        private readonly List<StringIdDefinition> _stringIds;   
           
        internal CacheHeader(CacheHeaderStruct s)
        {
            Unknown8 = s.Unknown8;
            AssetChecksum = s.AssetChecksum;
            Unknown18 = s.Unknown18;
            HeaderSize = s.HeaderSize;
            TagDataSize = s.TagDataSize;
            ResourceDataSize = s.ResourceDataSize;
            HeaderAlignment = s.HeaderAlignment;
            TagDataAlignment = s.TagDataAlignment;
            ResourceDataAlignment = s.ResourceDataAlignment;
            Unknown4B = s.Unknown4B;
            Unknown4C = s.Unknown4C;
            ZoneSetInfo = s.ZoneSetInfo;

            _dependencies = s.Dependencies.Select(ss => new TagDependency(ss, s.StringTable)).ToList();
            _dataBlocks = s.DataBlocks.Select(ss => new DataBlockDefinition(ss)).ToList();
            _tagStructs = s.TagStructs.Select(ss => new TagStructDefinition(ss)).ToList();
            _dataReferences = s.DataReferences.Select(ss => new DataReferenceDefinition(ss)).ToList();
            _tagReferences = s.TagReferences.Select(ss => new TagReferenceDefinition(ss, s.StringTable)).ToList();
            _stringIds = s.StringIds.Select(ss => new StringIdDefinition(ss, s.StringTable)).ToList();

            Dependencies = _dependencies.AsReadOnly();
            DataBlocks = _dataBlocks.AsReadOnly();
            TagStructs = _tagStructs.AsReadOnly();
            DataReferences = _dataReferences.AsReadOnly();
            TagReferences = _tagReferences.AsReadOnly();
            StringIds = _stringIds.AsReadOnly();
        }

        public static CacheHeader Read(Stream stream)
        {
            var headerStruct = BinarySerializer.Deserialize<CacheHeaderStruct>(stream);
            return new CacheHeader(headerStruct);
        }

        public ulong Unknown8 { get; }

        public ulong AssetChecksum { get; }

        public uint Unknown18 { get; }

        public uint HeaderSize { get; }

        public uint TagDataSize { get; }

        public uint ResourceDataSize { get; }

        public byte HeaderAlignment { get; }

        public byte TagDataAlignment { get; }

        public byte ResourceDataAlignment { get; }

        public byte Unknown4B { get; }

        public int Unknown4C { get; }

        public IList<TagDependency> Dependencies { get; }

        public IList<DataBlockDefinition> DataBlocks { get; }

        public IList<TagStructDefinition> TagStructs { get; } 

        public IList<DataReferenceDefinition> DataReferences { get; } 

        public IList<TagReferenceDefinition> TagReferences { get; } 

        public IList<StringIdDefinition> StringIds { get; }

        public byte[] ZoneSetInfo { get; }
    }
}

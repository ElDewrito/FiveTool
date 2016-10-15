using System.Collections.Generic;
using System.IO;
using System.Linq;
using FiveLib.Ausar.Memory.Stl;
using FiveLib.Common;

namespace FiveLib.Ausar.Memory.Tags
{
    using GlobalIdHashTable = StlUnorderedMap<HashableId32, MemoryTagInfo, HashableId32.Fnv1A64Hasher>;

    /// <summary>
    /// An in-memory hash map which maps global IDs to tag information.
    /// </summary>
    public class MemoryGlobalIdMap: IBinaryReadable, IBinaryWritable, IBinaryStruct
    {
        private readonly GlobalIdHashTable _hashTable = new GlobalIdHashTable();

        public MemoryGlobalIdMap()
        {
        }

        public MemoryGlobalIdMap(BinaryReader reader)
        {
            Read(reader);
        }

        public ulong Count => _hashTable.Count;

        public MemoryTagInfo GetTagInfo(uint globalId, BinaryReader reader)
        {
            MemoryTagInfo result;
            if (!_hashTable.TryGetValue(new HashableId32(globalId), reader, out result))
                throw new KeyNotFoundException("Global ID not found");
            return result;
        }

        public bool TryGetTagInfo(uint globalId, BinaryReader reader, out MemoryTagInfo result)
        {
            return _hashTable.TryGetValue(new HashableId32(globalId), reader, out result);
        }

        public IEnumerable<KeyValuePair<uint, MemoryTagInfo>> Enumerate(BinaryReader reader)
        {
            return _hashTable.Enumerate(reader).Select(p => new KeyValuePair<uint, MemoryTagInfo>(p.Key.Value, p.Value));
        }

        public void Read(BinaryReader reader) => _hashTable.Read(reader);

        public void Write(BinaryWriter writer) => _hashTable.Write(writer);

        public ulong GetStructSize() => _hashTable.GetStructSize();

        public ulong GetStructAlignment() => _hashTable.GetStructAlignment();
    }
}

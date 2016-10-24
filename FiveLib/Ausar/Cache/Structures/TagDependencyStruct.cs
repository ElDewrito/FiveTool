using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FiveLib.Common;
using FiveLib.IO;

namespace FiveLib.Ausar.Cache.Structures
{
    internal class TagDependencyStruct : IBinarySerializable
    {
        public MagicNumber GroupTag;
        public uint NameOffset;
        public ulong AssetId;
        public uint GlobalId;
        public int Unknown14;

        public void Serialize(BinarySerializer s)
        {
            s.Value(ref GroupTag);
            s.Value(ref NameOffset);
            s.Value(ref AssetId);
            s.Value(ref GlobalId);
            s.Value(ref Unknown14);
        }
    }
}

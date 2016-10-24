using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FiveLib.IO;

namespace FiveLib.Ausar.Cache.Structures
{
    internal class TagReferenceDefinitionStruct : IBinarySerializable
    {
        public int FieldBlockIndex;
        public uint FieldBlockOffset;
        public uint NameOffset;
        public int TagDependencyIndex;

        public void Serialize(BinarySerializer s)
        {
            s.Value(ref FieldBlockIndex);
            s.Value(ref FieldBlockOffset);
            s.Value(ref NameOffset);
            s.Value(ref TagDependencyIndex);
        }
    }
}

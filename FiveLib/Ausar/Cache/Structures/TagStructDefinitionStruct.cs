using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FiveLib.IO;

namespace FiveLib.Ausar.Cache.Structures
{
    internal class TagStructDefinitionStruct : IBinarySerializable
    {
        public byte[] Guid = new byte[0x10];
        public TagStructType Type;
        public short Unknown12;
        public int TargetIndex;
        public int FieldBlockIndex;
        public uint FieldBlockOffset;

        public void Serialize(BinarySerializer s)
        {
            s.Array(ref Guid, 0x10);
            s.Enum(ref Type);
            s.Value(ref Unknown12);
            s.Value(ref TargetIndex);
            s.Value(ref FieldBlockIndex);
            s.Value(ref FieldBlockOffset);
        }
    }
}

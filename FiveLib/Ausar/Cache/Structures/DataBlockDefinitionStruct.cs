using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FiveLib.IO;

namespace FiveLib.Ausar.Cache.Structures
{
    internal class DataBlockDefinitionStruct : IBinarySerializable
    {
        public uint Size;
        public short Unknown4;
        public DataBlockSection Section;
        public ulong Offset;

        public void Serialize(BinarySerializer s)
        {
            s.Value(ref Size);
            s.Value(ref Unknown4);
            s.Enum(ref Section);
            s.Value(ref Offset);
        }
    }
}

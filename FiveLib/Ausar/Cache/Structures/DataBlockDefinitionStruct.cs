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
        public short Unknown6;
        public ulong Offset;

        public void Serialize(BinarySerializer s)
        {
            s.Value(ref Size);
            s.Value(ref Unknown4);
            s.Value(ref Unknown6);
            s.Value(ref Offset);
        }
    }
}

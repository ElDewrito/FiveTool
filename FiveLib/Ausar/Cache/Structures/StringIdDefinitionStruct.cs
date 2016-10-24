using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FiveLib.IO;

namespace FiveLib.Ausar.Cache.Structures
{
    internal class StringIdDefinitionStruct : IBinarySerializable
    {
        public uint Unknown0;
        public uint StringOffset;

        public void Serialize(BinarySerializer s)
        {
            s.Value(ref Unknown0);
            s.Value(ref StringOffset);
        }
    }
}

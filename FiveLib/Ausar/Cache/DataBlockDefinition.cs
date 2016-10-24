using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FiveLib.Ausar.Cache.Structures;

namespace FiveLib.Ausar.Cache
{
    public class DataBlockDefinition
    {
        internal DataBlockDefinition(DataBlockDefinitionStruct s)
        {
            Size = s.Size;
            Unknown4 = s.Unknown4;
            Unknown6 = s.Unknown6;
            Offset = s.Offset;
        }

        public uint Size { get; }

        public short Unknown4 { get; }

        public short Unknown6 { get; }

        public ulong Offset { get; }
    }
}

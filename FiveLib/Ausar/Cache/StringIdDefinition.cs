using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FiveLib.Ausar.Cache.Structures;
using FiveLib.Common;

namespace FiveLib.Ausar.Cache
{
    public class StringIdDefinition
    {
        internal StringIdDefinition(StringIdDefinitionStruct s, StringBlob stringTable)
        {
            Unknown0 = s.Unknown0;
            String = stringTable.GetStringAtOffset((int)s.StringOffset);
        }

        public uint Unknown0 { get; }

        public string String { get; }
    }
}

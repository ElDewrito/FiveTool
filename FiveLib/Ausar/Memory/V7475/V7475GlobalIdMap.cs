using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FiveLib.Ausar.Memory.Tags;

namespace FiveLib.Ausar.Memory.V7475
{
    public static class V7475GlobalIdMap
    {
        public static MemoryGlobalIdMap Read(Process process, BinaryReader reader)
        {
            // TODO: Name these addresses
            reader.BaseStream.Position = (long)process.MainModule.BaseAddress + 0x5961180;
            reader.BaseStream.Position = reader.ReadInt64() + 8;
            reader.BaseStream.Position = reader.ReadInt64() + 0x98;
            return new MemoryGlobalIdMap(reader);
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FiveLib.Common;
using FiveLib.IO;

namespace FiveLib.Ausar.Module.Structures
{
    internal class ModuleStruct : IBinarySerializable
    {
        public ModuleFileHeaderStruct FileHeader = new ModuleFileHeaderStruct();
        public ModuleEntryStruct[] Entries = new ModuleEntryStruct[0];
        public StringBlob StringTable = new StringBlob(Encoding.UTF8);
        public int[] ResourceEntries = new int[0];
        public ModuleDataBlockStruct[] DataBlocks = new ModuleDataBlockStruct[0];

        public void Serialize(BinarySerializer s)
        {
            s.Object(ref FileHeader);
            s.Array(ref Entries, FileHeader.FileCount);
            s.StringTable(ref StringTable, Encoding.UTF8, (int)FileHeader.StringTableSize);
            s.Array(ref ResourceEntries, FileHeader.ResourceCount, (ss, i, v) => ss.Value(ref v));
            s.Array(ref DataBlocks, FileHeader.DataBlockCount);
        }
    }
}

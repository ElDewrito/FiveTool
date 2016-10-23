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
    internal class ModuleFileHeaderStruct : IBinarySerializable
    {
        public static readonly MagicNumber ExpectedMagic = new MagicNumber("dhom");
        public const int ExpectedVersion = 27;

        public MagicNumber Magic = ExpectedMagic;
        public int Version = ExpectedVersion;
        public ulong Id;
        public int FileCount;
        public int LoadedTagCount;
        public int FirstResourceIndex;
        public uint StringTableSize;
        public int ResourceCount;
        public int DataBlockCount;
        public ulong BuildVersionId;
        public ulong HeaderChecksum;

        public void Serialize(BinarySerializer s)
        {
            s.Expect(ss => ss.Value(ref Magic), nameof(Magic), ExpectedMagic);
            s.Expect(ss => ss.Value(ref Version), nameof(Version), ExpectedVersion);
            s.Value(ref Id);
            s.Value(ref FileCount);
            s.Value(ref LoadedTagCount);
            s.Value(ref FirstResourceIndex);
            s.Value(ref StringTableSize);
            s.Value(ref ResourceCount);
            s.Value(ref DataBlockCount);
            s.Value(ref BuildVersionId);
            s.Value(ref HeaderChecksum);
        }
    }
}

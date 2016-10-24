using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FiveLib.IO;

namespace FiveLib.Ausar.Cache.Structures
{
    internal class DataReferenceDefinitionStruct : IBinarySerializable
    {
        public int ParentStructIndex;
        public int Unknown4;
        public int TargetIndex;
        public int FieldBlockIndex;
        public uint FieldBlockOffset;

        public void Serialize(BinarySerializer s)
        {
            s.Value(ref ParentStructIndex);
            s.Value(ref Unknown4);
            s.Value(ref TargetIndex);
            s.Value(ref FieldBlockIndex);
            s.Value(ref FieldBlockOffset);
        }
    }
}

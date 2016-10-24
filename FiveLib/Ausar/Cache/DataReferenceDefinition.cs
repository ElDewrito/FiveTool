using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FiveLib.Ausar.Cache.Structures;

namespace FiveLib.Ausar.Cache
{
    public class DataReferenceDefinition
    {
        internal DataReferenceDefinition(DataReferenceDefinitionStruct s)
        {
            ParentStructIndex = s.ParentStructIndex;
            Unknown4 = s.Unknown4;
            TargetIndex = s.TargetIndex;
            FieldBlockIndex = s.FieldBlockIndex;
            FieldOffset = s.FieldBlockOffset;
        }

        public int ParentStructIndex { get; }

        public int Unknown4 { get; }

        public int TargetIndex { get; }

        public int FieldBlockIndex { get; }

        public uint FieldOffset { get; }
    }
}

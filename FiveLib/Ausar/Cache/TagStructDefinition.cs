using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FiveLib.Ausar.Cache.Structures;

namespace FiveLib.Ausar.Cache
{
    public class TagStructDefinition
    {
        internal TagStructDefinition(TagStructDefinitionStruct s)
        {
            Guid = s.Guid;
            Type = s.Type;
            Unknown12 = s.Unknown12;
            TargetIndex = s.TargetIndex;
            FieldBlockIndex = s.FieldBlockIndex;
            FieldBlockOffset = s.FieldBlockOffset;
        }

        public byte[] Guid { get; }

        public TagStructType Type { get; }

        public short Unknown12 { get; }

        public int TargetIndex { get; }

        public int FieldBlockIndex { get; }

        public uint FieldBlockOffset { get; }
    }
}

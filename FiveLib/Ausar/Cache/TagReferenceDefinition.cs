using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FiveLib.Ausar.Cache.Structures;
using FiveLib.Common;

namespace FiveLib.Ausar.Cache
{
    public class TagReferenceDefinition
    {
        internal TagReferenceDefinition(TagReferenceDefinitionStruct s, StringBlob stringTable)
        {
            FieldBlockIndex = s.FieldBlockIndex;
            FieldBlockOffset = s.FieldBlockOffset;
            Name = stringTable.GetStringAtOffset((int)s.NameOffset);
            TagDependencyIndex = s.TagDependencyIndex;
        }

        public int FieldBlockIndex { get; }

        public uint FieldBlockOffset { get; }

        public string Name { get; }

        public int TagDependencyIndex { get; }
    }
}

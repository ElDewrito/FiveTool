using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FiveLib.Ausar.Cache.Structures;
using FiveLib.Common;

namespace FiveLib.Ausar.Cache
{
    public class TagDependency
    {
        internal TagDependency(TagDependencyStruct s, StringBlob stringTable)
        {
            GroupTag = s.GroupTag;
            Name = stringTable.GetStringAtOffset((int)s.NameOffset);
            AssetId = s.AssetId;
            GlobalId = s.GlobalId;
            Unknown14 = s.Unknown14;
        }

        public MagicNumber GroupTag { get; }

        public string Name { get; }

        public ulong AssetId { get; }

        public uint GlobalId { get; }

        public int Unknown14 { get; }
    }
}

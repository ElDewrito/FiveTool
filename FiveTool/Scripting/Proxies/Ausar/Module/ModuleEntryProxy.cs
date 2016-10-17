using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FiveLib.Ausar.Module;
using MoonSharp.Interpreter;

namespace FiveTool.Scripting.Proxies.Ausar.Module
{
    [MoonSharpUserData]
    internal class ModuleEntryProxy
    {
        private readonly ModuleEntry _entry;

        [MoonSharpHidden]
        public ModuleEntryProxy(ModuleEntry entry)
        {
            _entry = entry;
        }

        public int Index => _entry.Index + 1;

        public string Name => _entry.Name;

        public int ParentIndex => _entry.ParentIndex != -1 ? _entry.ParentIndex + 1 : -1;

        public ListProxy<ModuleEntry> Resources => new ListProxy<ModuleEntry>(_entry.Resources); 

        public ListProxy<ModuleDataBlock> Blocks => new ListProxy<ModuleDataBlock>(_entry.Blocks);

        public long DataOffset => _entry.DataOffset;

        public uint TotalCompressedSize => _entry.TotalCompressedSize;

        public uint TotalUncompressedSize => _entry.TotalUncompressedSize;

        public byte HeaderAlignment => _entry.HeaderAlignment;

        public byte TagDataAlignment => _entry.TagDataAlignment;

        public byte ResourceDataAlignment => _entry.ResourceDataAlignment;

        public bool IsRawFile => _entry.IsRawFile;

        public uint GlobalId => _entry.GlobalId;

        public ulong AssetId => _entry.AssetId;

        public ulong AssetChecksum => _entry.AssetChecksum;

        public string GroupTag => _entry.GroupTag.Value != -1 ? _entry.GroupTag.ToString() : null;

        public uint UncompressedHeaderSize => _entry.UncompressedHeaderSize;

        public uint UncompressedTagDataSize => _entry.UncompressedTagDataSize;

        public uint UncompressedResourceDataSize => _entry.UncompressedResourceDataSize;

        public short HeaderBlockCount => _entry.HeaderBlockCount;

        public short TagDataBlockCount => _entry.TagDataBlockCount;

        public int ResourceBlockCount => _entry.ResourceBlockCount;

        public override string ToString() => $"(ModuleEntry \"{Name}\")";
    }
}

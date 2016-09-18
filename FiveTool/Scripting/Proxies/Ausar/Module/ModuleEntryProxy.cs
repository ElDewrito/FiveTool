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

        public string Name => _entry.Name;

        public int ParentIndex => _entry.ParentIndex;

        public int Unknown8 => _entry.Unknown8;

        public int UnknownC => _entry.UnknownC;

        public ListProxy<ModuleEntryBlock> Blocks => new ListProxy<ModuleEntryBlock>(_entry.Blocks);

        public ulong CompressedOffset => _entry.CompressedOffset;

        public uint TotalCompressedSize => _entry.TotalCompressedSize;

        public uint TotalUncompressedSize => _entry.TotalUncompressedSize;

        public byte Unknown28 => _entry.Unknown28;

        public byte Unknown29 => _entry.Unknown29;

        public byte Unknown2A => _entry.Unknown2A;

        public byte Unknown2B => _entry.Unknown2B;

        public int GlobalTagId => _entry.GlobalTagId;

        public long SourceTagId => _entry.SourceTagId;

        public long Unknown38 => _entry.Unknown38;

        public string GroupTag => _entry.GroupTag.Value != -1 ? _entry.GroupTag.ToString() : null;

        public uint UncompressedHeaderSize => _entry.UncompressedHeaderSize;

        public uint UncompressedTagDataSize => _entry.UncompressedTagDataSize;

        public uint UncompressedResourceDataSize => _entry.UncompressedResourceDataSize;

        public short Unknown50 => _entry.Unknown50;

        public short Unknown52 => _entry.Unknown52;

        public int Unknown54 => _entry.Unknown54;
    }
}

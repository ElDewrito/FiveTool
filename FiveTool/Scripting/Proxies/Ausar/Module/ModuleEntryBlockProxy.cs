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
    internal class ModuleEntryBlockProxy
    {
        private readonly ModuleEntryBlock _block;

        [MoonSharpHidden]
        public ModuleEntryBlockProxy(ModuleEntryBlock block)
        {
            _block = block;
        }

        public long Unknown0 => _block.Unknown0;

        public uint CompressedOffset => _block.CompressedOffset;

        public uint CompressedSize => _block.CompressedSize;

        public uint UncompressedOffset => _block.UncompressedOffset;

        public uint UncompressedSize => _block.UncompressedSize;

        public int Unknown18 => _block.Unknown18;

        public int Unknown1C => _block.Unknown1C;

        public override string ToString() => "(ModuleEntryBlock)";
    }
}

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
    internal class ModuleDataBlockProxy
    {
        private readonly ModuleDataBlock _block;

        [MoonSharpHidden]
        public ModuleDataBlockProxy(ModuleDataBlock block)
        {
            _block = block;
        }

        public UInt64Proxy Checksum => new UInt64Proxy(_block.Checksum);

        public uint CompressedOffset => _block.CompressedOffset;

        public uint CompressedSize => _block.CompressedSize;

        public uint UncompressedOffset => _block.UncompressedOffset;

        public uint UncompressedSize => _block.UncompressedSize;

        public bool IsCompressed => _block.IsCompressed;

        public override string ToString() => "(ModuleDataBlock)";
    }
}

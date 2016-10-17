using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FiveLib.Ausar.Module;
using FiveTool.Scripting.Platform;
using MoonSharp.Interpreter;

namespace FiveTool.Scripting.Proxies.Ausar.Module
{
    [MoonSharpUserData]
    internal class AusarModuleProxy
    {
        private readonly AusarModule _module;
        private readonly FileInfo _file;

        [MoonSharpHidden]
        public AusarModuleProxy(AusarModule module, FileInfo file)
        {
            _module = module;
            _file = file;
        }

        public UInt64Proxy Id => new UInt64Proxy(_module.Id);

        public int LoadedTagCount => _module.LoadedTagCount;

        public UInt64Proxy BuildVersionId => new UInt64Proxy(_module.BuildVersionId);

        public ListProxy<ModuleEntry> Entries => new ListProxy<ModuleEntry>(_module.Entries);

        public long DataBaseOffset => _module.DataBaseOffset;

        public ModuleEntry GetEntryByName(string name)
        {
            ModuleEntry result;
            _module.GetEntryByName(name, out result);
            return result;
        }

        public ModuleEntry GetEntryByGlobalTagId(uint id)
        {
            ModuleEntry result;
            _module.GetEntryByGlobalTagId(id, out result);
            return result;
        }

        public static AusarModuleProxy LoadFromFile(string path)
        {
            path = FileSandbox.ResolvePath(path);
            var file = new FileInfo(path);
            using (var stream = file.OpenRead())
                return new AusarModuleProxy(AusarModule.Open(stream), file);
        }

        public static UInt64Proxy ReadId(string path)
        {
            path = FileSandbox.ResolvePath(path);
            using (var stream = File.OpenRead(path))
                return new UInt64Proxy(AusarModule.ReadId(stream));
        }

        public override string ToString() => $"(AusarModule, {Entries.Count} entries)";
    }
}

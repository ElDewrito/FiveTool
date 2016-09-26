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
    internal class AusarModuleProxy : IDisposable
    {
        private readonly AusarModule _module;
        private Stream _stream;

        [MoonSharpHidden]
        public AusarModuleProxy(AusarModule module)
        {
            _module = module;
        }

        [MoonSharpHidden]
        public AusarModuleProxy(AusarModule module, Stream stream)
            : this(module)
        {
            _stream = stream;
        }

        public ListProxy<ModuleEntry> Entries => new ListProxy<ModuleEntry>(_module.Entries);

        public ListProxy<ModuleEntry> Resources => new ListProxy<ModuleEntry>(_module.Resources);

        public long DataBaseOffset => _module.DataBaseOffset;

        public ModuleEntry GetEntryByName(string name)
        {
            ModuleEntry result;
            _module.GetEntryByName(name, out result);
            return result;
        }

        public ModuleEntry GetEntryByGlobalTagId(int id)
        {
            ModuleEntry result;
            _module.GetEntryByGlobalTagId(id, out result);
            return result;
        }

        public static AusarModuleProxy LoadFromFile(string path)
        {
            path = FileSandbox.ResolvePath(path);
            var stream = File.Open(path, FileMode.Open, FileAccess.ReadWrite);
            var module = AusarModule.Open(stream);
            return new AusarModuleProxy(module, stream);
        }

        public override string ToString() => $"(AusarModule, {Entries.Count} entries)";

        ~AusarModuleProxy()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_stream != null)
                {
                    _stream.Dispose();
                    _stream = null;
                }
            }
        }
    }
}

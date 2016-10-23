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

        public bool ContainsEntry(ModuleEntry entry)
        {
            return _module.ContainsEntry(entry);
        }

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

        public void ExtractEntry(ModuleEntry entry, string path, string sectionName)
        {
            if (!_module.ContainsEntry(entry))
                throw new ScriptRuntimeException("Entry is not contained inside the module");
            path = FileSandbox.ResolvePath(path);
            var section = TranslateSection(sectionName);
            using (var stream = _file.OpenRead())
                _module.ExtractEntry(stream, entry, section, path);
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

        private static ModuleEntrySection TranslateSection(string sectionName)
        {
            if (sectionName == null)
                return ModuleEntrySection.All;
            switch (sectionName.ToUpperInvariant())
            {
                case "HEADER":
                    return ModuleEntrySection.Header;
                case "TAG":
                    return ModuleEntrySection.TagData;
                case "RESOURCE":
                    return ModuleEntrySection.ResourceData;
                case "ALL":
                    return ModuleEntrySection.All;
                default:
                    throw new ScriptRuntimeException($"Unsupported module entry section \"${sectionName}\"");
            }
        }

        public override string ToString() => $"(AusarModule, {Entries.Count} entries)";
    }
}

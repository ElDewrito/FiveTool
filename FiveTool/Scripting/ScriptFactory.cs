using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FiveLib.Ausar.Module;
using FiveTool.Scripting.Proxies.Ausar.Module;
using MoonSharp.Interpreter;

namespace FiveTool.Scripting
{
    internal static class ScriptFactory
    {
        public static void Initialize()
        {
            UserData.RegisterAssembly();
            UserData.RegisterProxyType<AusarModuleProxy, AusarModule>(r => new AusarModuleProxy(r));
            UserData.RegisterProxyType<ModuleEntryProxy, ModuleEntry>(r => new ModuleEntryProxy(r));
            UserData.RegisterProxyType<ModuleEntryBlockProxy, ModuleEntryBlock>(r => new ModuleEntryBlockProxy(r));
        }

        public static Script CreateScript()
        {
            var script = new Script(CoreModules.Preset_SoftSandbox);

            script.Globals["Module"] = UserData.CreateStatic(typeof(AusarModuleProxy));

            return script;
        }
    }
}

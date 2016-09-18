using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FiveLib.Ausar.Module;
using FiveTool.Scripting.Proxies.Ausar.Module;
using MoonSharp.Interpreter;

namespace FiveTool.Scripting
{
    internal static class ScriptFactory
    {
        private const string BuiltInsNamespace = "BuiltIns";

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

            RunBuiltIns(script);
            return script;
        }

        private static void RunBuiltIns(Script script)
        {
            // Find all lua files in <namespace>.BuiltIns
            var assembly = Assembly.GetExecutingAssembly();
            var resources = assembly.GetManifestResourceNames();
            var ns = typeof(ScriptFactory).Namespace + "." + BuiltInsNamespace;
            foreach (var resource in resources.Where(r => r.StartsWith(ns) && r.EndsWith(".lua")))
            {
                using (var stream = assembly.GetManifestResourceStream(resource))
                {
                    var loaded = script.LoadStream(stream, null, resource);
                    script.Call(loaded);
                }
            }
        }
    }
}

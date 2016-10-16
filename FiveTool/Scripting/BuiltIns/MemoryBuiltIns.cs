using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FiveLib.Ausar.Memory.Tags;
using FiveLib.Ausar.Memory.V7475;
using FiveLib.Memory;
using MoonSharp.Interpreter;

namespace FiveTool.Scripting.BuiltIns
{
    public class MemoryBuiltIns
    {
        private const string ProcessName = "halo5forge";
        private const string MemoryModuleName = "Memory_Release_UWP.dll"; // Used to quickly verify if a process is actually H5

        private static Process _gameProcess;

        public static void Register(Script script)
        {
            script.Globals["MemGetLocalTagHandle"] = (Func<uint, long>)MemGetLocalTagHandle;
            script.Globals["MemGetLoadedTags"] = DynValue.NewCallback(MemGetLoadedTags, nameof(MemGetLoadedTags));
            script.Globals["MemGetTagAddressFromHandle"] = (Func<uint, ulong>)MemGetTagAddressFromHandle;
        }

        private static long MemGetLocalTagHandle(uint globalId)
        {
            try
            {
                var process = GetGameProcess();
                if (process == null)
                    return -1;
                using (var memoryReader = new BinaryReader(new ProcessMemoryStream(process, true)))
                {
                    var tags = V7475GlobalIdMap.Read(process, memoryReader);
                    MemoryTagInfo info;
                    if (!tags.TryGetTagInfo(globalId, memoryReader, out info))
                        return -1;
                    return info.LocalHandle;
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine(ex);
#endif
                return -1;
            }
        }

        private static DynValue MemGetLoadedTags(ScriptExecutionContext context, CallbackArguments args)
        {
            var script = context.GetScript();
            try
            {
                var table = DynValue.NewTable(script);
                var process = GetGameProcess();
                if (process == null)
                    return table;
                using (var memoryReader = new BinaryReader(new ProcessMemoryStream(process, true)))
                {
                    var tags = V7475GlobalIdMap.Read(process, memoryReader);
                    foreach (var pair in tags.Enumerate(memoryReader))
                        table.Table.Set(DynValue.NewNumber(pair.Key), DynValue.NewNumber(pair.Value.LocalHandle));
                }
                return table;
            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine(ex);
#endif
                return DynValue.NewTable(script);
            }
        }

        private static ulong MemGetTagAddressFromHandle(uint localHandle)
        {
            try
            {
                var process = GetGameProcess();
                if (process == null)
                    return 0;
                using (var memoryReader = new BinaryReader(new ProcessMemoryStream(process, true)))
                {
                    var addresses = V7475TagAddressList.Read(process, memoryReader);
                    ulong address;
                    return addresses.TryGetTagAddress(localHandle, memoryReader, out address) ? address : 0;
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine(ex);
#endif
                return 0;
            }
        }

        private static Process GetGameProcess()
        {
            if (_gameProcess != null && !_gameProcess.HasExited)
                return _gameProcess;

            // Find the first halo5forge process that has Memory_Release_UWP.dll loaded
            var processes = Process.GetProcessesByName(ProcessName);
            _gameProcess = processes.FirstOrDefault(p => p.Modules.Cast<ProcessModule>().Any(m => m.ModuleName == MemoryModuleName));
            return _gameProcess;
        }
    }
}

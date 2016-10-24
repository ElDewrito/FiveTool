using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FiveTool.Scripting.Proxies;
using FiveTool.Scripting.Proxies.Ausar.Tags;
using MoonSharp.Interpreter;

namespace FiveTool.Scripting.BuiltIns
{
    public static class DataTypeBuiltIns
    {
        public static void Register(Script script)
        {
            script.Globals["StringIdFactory"] = UserData.CreateStatic<StringIdProxy>();
            script.Globals["UInt64Factory"] = UserData.CreateStatic<UInt64Proxy>();
            script.Globals["BSwap32"] = (Func<uint, uint>)BSwap32;
        }

        private static uint BSwap32(uint val)
        {
            return ((val << 24) & 0xFF000000) | ((val << 8) & 0xFF0000) | ((val >> 8) & 0xFF00) | ((val >> 24) & 0xFF);
        }
    }
}

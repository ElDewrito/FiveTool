using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FiveLib.Ausar.Tags;
using FiveTool.Scripting.Proxies.Ausar.Tags;
using MoonSharp.Interpreter;

namespace FiveTool.Scripting.BuiltIns
{
    public static class DataTypeBuiltIns
    {
        public static void Register(Script script)
        {
            script.Globals["StringId"] = UserData.CreateStatic<StringIdProxy>();
        }
    }
}

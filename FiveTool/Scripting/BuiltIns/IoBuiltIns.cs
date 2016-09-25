using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FiveTool.Scripting.Platform;
using MoonSharp.Interpreter;

namespace FiveTool.Scripting.BuiltIns
{
    internal static class IoBuiltIns
    {
        public static void Register(Script script)
        {
            script.Globals["ChooseGameFolder"] = (Func<bool>)FileAccessUtil.ChooseGameFolder;
        }
    }
}

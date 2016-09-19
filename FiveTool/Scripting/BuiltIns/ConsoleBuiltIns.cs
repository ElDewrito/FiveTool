using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoonSharp.Interpreter;

namespace FiveTool.Scripting.BuiltIns
{
    internal static class ConsoleBuiltIns
    {
        public static void Register(Script script)
        {
            script.Globals["Clear"] = (Action)Clear;
            script.Globals["Color"] = (Action<string>)SetColor;
        }

        private static void Clear()
        {
            Console.Clear();
        }

        private static readonly Dictionary<string, ConsoleColor> Colors = new Dictionary<string, ConsoleColor>
        {
            { "Black", ConsoleColor.Black },
            { "Blue", ConsoleColor.Blue },
            { "Cyan", ConsoleColor.Cyan },
            { "DarkBlue", ConsoleColor.DarkBlue },
            { "DarkCyan", ConsoleColor.DarkCyan },
            { "DarkGray", ConsoleColor.DarkGray },
            { "DarkGreen", ConsoleColor.DarkGreen },
            { "DarkMagenta", ConsoleColor.DarkMagenta },
            { "DarkRed", ConsoleColor.DarkRed },
            { "DarkYellow", ConsoleColor.DarkYellow },
            { "Gray", ConsoleColor.Gray },
            { "Green", ConsoleColor.Green },
            { "Magenta", ConsoleColor.Magenta },
            { "Red", ConsoleColor.Red },
            { "White", ConsoleColor.White },
            { "Yellow", ConsoleColor.Yellow },
        };

        private static void SetColor(string name)
        {
            ConsoleColor color;
            if (!Colors.TryGetValue(name, out color))
                throw new ScriptRuntimeException($"Unrecognized color name \"{name}\"");
            Console.ForegroundColor = color;
        }
    }
}

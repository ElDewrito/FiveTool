using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FiveTool.Scripting;
using MoonSharp.Interpreter;

namespace FiveTool
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.White;
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            Console.WriteLine("FiveTool [{0}.{1}.{2}]", version.Major, version.Minor, version.Revision);
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("Please submit bug reports, pull requests, and feature requests to");
            Console.WriteLine("<https://github.com/Shockfire/FiveTool>.");
            Console.WriteLine();

            Console.WriteLine("Starting interactive Lua (MoonSharp) shell.");
            Console.Write("Use ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("Help()");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write(" for help and ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("Exit()");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine(" to quit.");
            Console.WriteLine();

            ScriptFactory.Initialize();
            var script = ScriptFactory.CreateScript();

            var done = false;
            script.Globals["Exit"] = (Action)(() => { done = true; });

            while (!done)
            {
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.Write("lua> ");
                Console.ForegroundColor = ConsoleColor.White;
                var line = Console.ReadLine();
                Console.ForegroundColor = ConsoleColor.Gray;
                if (string.IsNullOrWhiteSpace(line))
                    continue;
                try
                {
                    // HACK: Stick a return statement in front of the line first, and then try without it if there's a syntax error
                    // Otherwise users would need to manually type "return" in front of most lines
                    DynValue result;
                    try
                    {
                        result = script.DoString("return " + line);
                    }
                    catch (SyntaxErrorException)
                    {
                        result = script.DoString(line);
                    }
                    if (result.IsNotVoid())
                        Console.WriteLine(result.ToString());
                }
                catch (InterpreterException e)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(e.DecoratedMessage);
                }
                catch (Exception e)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(e.Message);
                }
                Console.WriteLine();
            }
        }
    }
}

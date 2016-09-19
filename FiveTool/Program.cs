using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FiveTool.Scripting;
using FiveTool.Scripting.BuiltIns;
using MoonSharp.Interpreter;

namespace FiveTool
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length > 1)
            {
                Console.WriteLine("Usage: FiveTool [script path]");
                return;
            }

            var scriptPath = args.Length > 0 ? args[0] : null;
            if (scriptPath == null)
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
            }

            ScriptFactory.Initialize();
            var script = ScriptFactory.CreateScript();

            // If a script file was passed in, run it and return
            if (scriptPath != null)
            {
                try
                {
                    script.DoFile(scriptPath);
                }
                catch (InterpreterException e)
                {
                    Console.Error.WriteLine("Error: " + e.DecoratedMessage);
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine("Error: " + e.Message);
                }
                return;
            }

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
                    {
                        // Dump without recursion to display the value in a friendly format
                        DumpBuiltIns.Dump(script, result, 0);
                        Console.WriteLine();
                    }
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

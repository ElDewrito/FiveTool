using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FiveTool.Scripting;
using FiveTool.Scripting.BuiltIns;
using FiveTool.Scripting.Platform;
using MoonSharp.Interpreter;

namespace FiveTool
{
    internal class Program
    {
        [STAThread]
        private static void Main(string[] args)
        {
            if (args.Length > 1)
            {
                Console.WriteLine("Usage: FiveTool [script path]");
                return;
            }

            var scriptPath = args.Length > 0 ? args[0] : null;
            var configPath = Config.DefaultPath;
            var consoleMode = scriptPath == null;

            if (!LoadConfig(configPath))
                Console.Error.WriteLine($"Failed to load configuration data from {configPath}! Using defaults.");

            if (consoleMode)
            {
                Console.ForegroundColor = ConsoleColor.White;
                var version = Assembly.GetExecutingAssembly().GetName().Version;
                Console.WriteLine("FiveTool [{0}.{1}.{2}]", version.Major, version.Minor, version.Revision);
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine("Please submit bug reports, pull requests, and feature requests to");
                Console.WriteLine("<https://github.com/Shockfire/FiveTool>.");
                Console.WriteLine();
            }

            if (EnsureGameRootIsSet())
            {
                if (consoleMode)
                {
                    Console.WriteLine($"Using game files in {Config.Current.GameRoot}.");
                    Console.Write("You can use ");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write("ChooseGameFolder()");
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.WriteLine(" to change this folder at any time.");
                    Console.WriteLine();
                }
            }
            else
            {
                Console.Error.WriteLine("You must set a valid game folder in order to use FiveTool.");
                if (consoleMode)
                {
                    Console.WriteLine("Press any key to quit...");
                    Console.ReadKey(true);
                }
                return;
            }

            if (consoleMode)
            {
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

            // If a script file was passed in, run it
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
            }
            if (!consoleMode)
                return;

            var done = false;
            script.Globals["Exit"] = (Action)(() => { done = true; });

            while (!done)
            {
                try
                {
                    var func = ReadFunction(script);
                    var result = func.Function.Call();
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

        private static bool LoadConfig(string path)
        {
            try
            {
                if (File.Exists(path))
                    Config.Current = Config.Load(path);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static bool EnsureGameRootIsSet()
        {
            if (string.IsNullOrWhiteSpace(Config.Current.GameRoot) || !Directory.Exists(Config.Current.GameRoot))
            {
                Console.Error.WriteLine("Halo 5: Forge root folder is invalid or not set. Please select a new one.");
                if (!Dialogs.ChooseGameFolder())
                    return false;
            }
            return true;
        }

        private static DynValue ReadFunction(Script script)
        {
            var scriptStr = new StringBuilder();
            while (true)
            {
                if (scriptStr.Length == 0)
                {
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.Write("lua> ");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.Write(".... ");
                }

                Console.ForegroundColor = ConsoleColor.White;
                var line = Console.ReadLine();
                Console.ForegroundColor = ConsoleColor.Gray;

                // Reset the script if the line is blank
                if (string.IsNullOrWhiteSpace(line))
                {
                    scriptStr.Clear();
                    Console.WriteLine();
                    continue;
                }

                scriptStr.AppendLine(line);

                // HACK: Stick a return statement in front of the line first, and then try without it if there's a syntax error
                // Otherwise users would need to manually type "return" in front of most lines
                try
                {
                    return script.LoadString("return " + scriptStr, null, "console");
                }
                catch (SyntaxErrorException e)
                {
                    if (e.IsPrematureStreamTermination)
                        continue; // Need more input
                }
                try
                {
                    return script.LoadString(scriptStr.ToString(), null, "console");
                }
                catch (SyntaxErrorException e)
                {
                    if (!e.IsPrematureStreamTermination)
                        throw; // Enough input was supplied, re-throw the exception
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoonSharp.Interpreter;

namespace FiveTool.Scripting.BuiltIns
{
    internal static class IoBuiltIns
    {
        public static void Register(Script script)
        {
            // TODO: Do some sandboxing so that the official implementations of these functions can be used
            var ioTable = new Table(script)
            {
                ["read"] = (Func<DynValue, DynValue>)Read,
                ["write"] = DynValue.NewCallback(Write, nameof(Write)),
                ["flush"] = (Action)Flush,
            };
            script.Globals["io"] = ioTable;
        }

        private static DynValue Read(DynValue format)
        {
            if (format.IsNil())
                return DynValue.NewString(Console.ReadLine());
            if (format.Type == DataType.String)
            {
                var formatStr = format.String;
                switch (formatStr)
                {
                    case "n":
                        // Read number
                        int result;
                        if (!int.TryParse(Console.ReadLine(), out result))
                            return DynValue.Nil;
                        return DynValue.NewNumber(result);
                    case "a":
                        // Read everything until EOF
                        var builder = new StringBuilder();
                        while (true)
                        {
                            var line = Console.ReadLine();
                            if (line == null)
                                break;
                            builder.AppendLine(line);
                        }
                        return DynValue.NewString(builder);
                    case "l":
                        // Read a line
                        return DynValue.NewString(Console.ReadLine());
                    case "L":
                        // Read a line and newline
                        return DynValue.NewString(Console.ReadLine() + "\n");
                }
            }
            else if (format.Type == DataType.Number)
            {
                // Read a number of characters
                var count = (int)format.Number;
                var buffer = new char[count];
                var numRead = Console.In.Read(buffer, 0, count);
                return DynValue.NewString(new string(buffer, 0, numRead));
            }
            throw new ScriptRuntimeException($"Invalid io.read() format \"{format}\"");
        }

        private static DynValue Write(ScriptExecutionContext context, CallbackArguments args)
        {
            for (var i = 0; i < args.Count; i++)
                Console.Write(args[i].CastToString());
            return DynValue.Void;
        }

        private static void Flush()
        {
            Console.Out.Flush();
        }
    }
}

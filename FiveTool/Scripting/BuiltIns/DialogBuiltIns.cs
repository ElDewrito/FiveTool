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
    internal static class DialogBuiltIns
    {
        public static void Register(Script script)
        {
            var dialog = new Table(script)
            {
                ["OpenFile"] = DynValue.NewCallback(OpenFile, nameof(OpenFile)),
                ["OpenGameFolder"] = (Func<bool>)OpenGameFolder,
            };
            script.Globals["Dialog"] = dialog;
        }

        private static DynValue OpenFile(ScriptExecutionContext context, CallbackArguments args)
        {
            var options = args.AsType(0, "OpenFile", DataType.Table).Table;
            var type = options.Get("type").CastToString();
            var title = options.Get("title").CastToString();
            var filter = ParseFiltersTable(options.Get("filters"));
            var suggestedNameValue = options.Get("suggestedName");
            var suggestedName = !suggestedNameValue.IsNil() ? suggestedNameValue.CastToString() : "";

            bool accepted;
            string selectedFile;
            switch (type)
            {
                case "open":
                    accepted = Dialogs.OpenFile(title, filter, out selectedFile);
                    break;
                case "save":
                    accepted = Dialogs.SaveFile(title, filter, suggestedName, out selectedFile);
                    break;
                default:
                    throw new ScriptRuntimeException($"Unsupported OpenFile dialog type \"{type}\"");
            }
            if (!accepted)
                return DynValue.Nil;
            var token = PathToken.Generate(selectedFile);
            return DynValue.NewString(token);
        }

        private static bool OpenGameFolder()
        {
            return Dialogs.ChooseGameFolder();
        }

        private static string ParseFiltersTable(DynValue table)
        {
            if (table.Type != DataType.Table)
                throw new ScriptRuntimeException("filters must be in a table");
            var filter = new StringBuilder();
            foreach (var filterPair in table.Table.Values)
            {
                if (filterPair.Type != DataType.Table)
                    continue;
                var name = filterPair.Table.Get(1).CastToString();
                var pattern = filterPair.Table.Get(2).CastToString();
                if (filter.Length > 0)
                    filter.Append("|");
                filter.AppendFormat("{0}|{1}", name, pattern);
            }
            return filter.ToString();
        }
    }
}

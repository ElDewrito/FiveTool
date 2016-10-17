using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FiveTool.Scripting.Proxies;
using FiveTool.Scripting.Proxies.Ausar.Tags;
using MoonSharp.Interpreter;

namespace FiveTool.Scripting.BuiltIns
{
    internal static class DumpBuiltIns
    {
        public static void Register(Script script)
        {
            script.Globals["Dump"] = DynValue.NewCallback(Dump, nameof(Dump));
        }

        public static void Dump(Script script, DynValue val, int maxDepth = -1)
        {
            Dump(script, val, 0, maxDepth);
        }

        private static DynValue Dump(ScriptExecutionContext context, CallbackArguments args)
        {
            var val = args.RawGet(0, true);
            if (val == null)
                throw new ScriptRuntimeException("Missing a value to dump");
            var maxDepthVal = args.RawGet(1, true);
            var maxDepth = maxDepthVal != null ? (int)(maxDepthVal.CastToNumber() ?? -1) : -1;
            Dump(context.GetScript(), val, maxDepth);
            Console.WriteLine();
            return DynValue.Void;
        }

        private static void Dump(Script script, DynValue val, int depth, int maxDepth)
        {
            switch (val.Type)
            {
                case DataType.ClrFunction:
                    Console.Write("(Built-in function {0})", val.Callback.Name);
                    break;
                case DataType.Table:
                    DumpTable(script, val.Table, depth, maxDepth);
                    break;
                case DataType.UserData:
                    DumpUserData(script, val.UserData, depth, maxDepth);
                    break;
                default:
                    Console.Write(val.ToString());
                    break;
            }
        }

        private static void DumpTable(Script script, Table table, int depth, int maxDepth)
        {
            if (depth == maxDepth)
            {
                // TODO: Is there a good way to get the key count without enumerating everything?
                Console.Write("(Table)");
                return;
            }

            Console.Write("{");
            var first = true;
            foreach (var pair in table.Pairs)
            {
                if (!first)
                    Console.WriteLine(",");
                else
                    Console.WriteLine();
                first = false;

                Indent(depth + 1);
                Console.ForegroundColor = ConsoleColor.White;
                Dump(script, pair.Key, depth + 1, maxDepth);
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write(" = ");
                Dump(script, pair.Value, depth + 1, maxDepth);
            }
            Console.WriteLine();
            Indent(depth);
            Console.Write("}");
        }

        private static void DumpUserData(Script script, UserData data, int depth, int maxDepth)
        {
            var obj = data.Object;
            var type = data.Descriptor.Type;
            var originalType = type;

            if (obj == null)
            {
                Console.Write($"(Static {type.Name})");
                return;
            }

            // If this is a proxy type, convert it
            obj = ProxyUtil.GetUserDataAsProxy(data);
            type = obj.GetType();

            // Special cases
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ListProxy<>))
            {
                DumpList(script, type, obj, depth, maxDepth);
                return;
            }
            if (type == typeof(StringIdProxy))
            {
                Console.Write($"(StringId {obj})");
                return;
            }
            if (type == typeof(UInt64Proxy))
            {
                Console.Write($"(UInt64 {obj})");
                return;
            }

            if (depth == maxDepth)
            {
                Console.Write(obj);
                return;
            }

            Console.Write($"{originalType.Name} {{");

            // Enumerate over public fields and properties
            var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
            var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
            var first = true;
            foreach (var member in fields.Concat<MemberInfo>(properties))
            {
                object val;
                switch (member.MemberType)
                {
                    case MemberTypes.Field:
                        val = ((FieldInfo)member).GetValue(obj);
                        break;
                    case MemberTypes.Property:
                        var property = (PropertyInfo)member;
                        if (property.GetMethod.GetParameters().Length > 0)
                            continue; // Ignore indexers, etc.
                        val = property.GetValue(obj);
                        break;
                    default:
                        continue;
                }

                if (!first)
                    Console.WriteLine(",");
                else
                    Console.WriteLine();
                first = false;

                Indent(depth + 1);
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(member.Name);
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write(" = ");
                Dump(script, DynValue.FromObject(script, val), depth + 1, maxDepth);
            }
            Console.WriteLine();
            Indent(depth);
            Console.Write("}");
        }

        private static void DumpList(Script script, Type type, object list, int depth, int maxDepth)
        {
            var countProperty = type.GetProperty("Count");
            var itemProperty = type.GetProperty("Item");
            var count = (int)countProperty.GetValue(list);

            var elementTypeName = type.GenericTypeArguments[0].Name;
            if (depth == maxDepth)
            {
                Console.Write($"({elementTypeName}[{count}])");
                return;
            }

            Console.Write($"{elementTypeName}[{count}] {{");
            for (var i = 1; i <= count; i++)
            {
                if (i > 1)
                    Console.WriteLine(",");
                else
                    Console.WriteLine();

                var val = itemProperty.GetValue(list, new object[] { i });

                Indent(depth + 1);
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(i);
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write(" = ");
                Dump(script, DynValue.FromObject(script, val), depth + 1, maxDepth);
            }
            Console.WriteLine();
            Indent(depth);
            Console.Write("}");
        }

        private static void Indent(int indentLevel)
        {
            for (var i = 0; i < indentLevel; i++)
                Console.Write("  ");
        }
    }
}

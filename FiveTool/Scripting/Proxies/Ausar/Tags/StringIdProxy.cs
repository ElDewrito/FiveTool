using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FiveLib.Ausar.Tags;
using MoonSharp.Interpreter;

namespace FiveTool.Scripting.Proxies.Ausar.Tags
{
    [MoonSharpUserData]
    public class StringIdProxy : IComparable<StringIdProxy>
    {
        private readonly StringId _stringId;

        public StringIdProxy(StringId stringId)
        {
            _stringId = stringId;
        }

        public static StringIdProxy FromString(string str)
        {
            return new StringIdProxy(new StringId(str));
        }

        public static StringIdProxy FromNumber(uint num)
        {
            return new StringIdProxy(new StringId(num));
        }

        public uint Value => _stringId.Value;

        public int CompareTo(StringIdProxy other)
        {
            return _stringId.CompareTo(other._stringId);
        }

        public override string ToString() => _stringId.ToString();

        public override bool Equals(object obj)
        {
            var other = obj as StringIdProxy;
            if (other == null)
                return false;
            return _stringId == other._stringId;
        }

        public override int GetHashCode() => _stringId.GetHashCode();
    }
}

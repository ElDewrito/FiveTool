using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoonSharp.Interpreter;

namespace FiveTool.Scripting.Proxies
{
    /// <summary>
    /// Proxies a 64-bit unsigned integer value so that precision isn't lost by it being converted to double.
    /// </summary>
    [MoonSharpUserData]
    internal class UInt64Proxy : IComparable
    {
        public static readonly UInt64Proxy Zero = new UInt64Proxy(0);

        public UInt64Proxy(ulong value)
        {
            Value = value;
        }

        public static UInt64Proxy FromNumber(ulong value)
        {
            return new UInt64Proxy(value);
        }

        public static UInt64Proxy FromString(string str)
        {
            try
            {
                if (str.StartsWith("0x"))
                    return new UInt64Proxy(ulong.Parse(str.Substring(2), NumberStyles.HexNumber));
                return new UInt64Proxy(ulong.Parse(str));
            }
            catch (Exception e)
            {
                throw new ScriptRuntimeException(e);
            }
        }

        public ulong Value { get; }

        public static UInt64Proxy operator +(UInt64Proxy lhs, UInt64Proxy rhs)
        {
            return new UInt64Proxy(lhs.Value + rhs.Value);
        }

        public static UInt64Proxy operator +(UInt64Proxy lhs, ulong rhs)
        {
            return new UInt64Proxy(lhs.Value + rhs);
        }

        public static UInt64Proxy operator +(ulong lhs, UInt64Proxy rhs)
        {
            return new UInt64Proxy(lhs + rhs.Value);
        }

        public static UInt64Proxy operator -(UInt64Proxy lhs, UInt64Proxy rhs)
        {
            return new UInt64Proxy(lhs.Value - rhs.Value);
        }

        public static UInt64Proxy operator -(UInt64Proxy lhs, ulong rhs)
        {
            return new UInt64Proxy(lhs.Value - rhs);
        }

        public static UInt64Proxy operator -(ulong lhs, UInt64Proxy rhs)
        {
            return new UInt64Proxy(lhs - rhs.Value);
        }

        public static UInt64Proxy operator *(UInt64Proxy lhs, UInt64Proxy rhs)
        {
            return new UInt64Proxy(lhs.Value * rhs.Value);
        }

        public static UInt64Proxy operator *(UInt64Proxy lhs, ulong rhs)
        {
            return new UInt64Proxy(lhs.Value * rhs);
        }

        public static UInt64Proxy operator *(ulong lhs, UInt64Proxy rhs)
        {
            return new UInt64Proxy(lhs * rhs.Value);
        }

        public static UInt64Proxy operator /(UInt64Proxy lhs, UInt64Proxy rhs)
        {
            return new UInt64Proxy(lhs.Value / rhs.Value);
        }

        public static UInt64Proxy operator /(UInt64Proxy lhs, ulong rhs)
        {
            return new UInt64Proxy(lhs.Value / rhs);
        }

        public static UInt64Proxy operator /(ulong lhs, UInt64Proxy rhs)
        {
            return new UInt64Proxy(lhs / rhs.Value);
        }

        public static UInt64Proxy operator %(UInt64Proxy lhs, UInt64Proxy rhs)
        {
            return new UInt64Proxy(lhs.Value % rhs.Value);
        }

        public static UInt64Proxy operator %(UInt64Proxy lhs, ulong rhs)
        {
            return new UInt64Proxy(lhs.Value % rhs);
        }

        public static UInt64Proxy operator %(ulong lhs, UInt64Proxy rhs)
        {
            return new UInt64Proxy(lhs % rhs.Value);
        }

        public int CompareTo(object obj)
        {
            var otherU64 = obj as UInt64Proxy;
            if (otherU64 != null)
                return Value.CompareTo(otherU64.Value);
            try
            {
                return Value.CompareTo(Convert.ToUInt64(obj));
            }
            catch (Exception ex)
            {
                throw new ScriptRuntimeException(ex);
            }
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            var otherU64 = obj as UInt64Proxy;
            if (otherU64 != null)
                return Value == otherU64.Value;
            try
            {
                return Value == Convert.ToUInt64(obj);
            }
            catch
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public string ToHexString() => Value.ToString("x");

        public override string ToString() => Value.ToString();
    }
}

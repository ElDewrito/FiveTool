using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Murmur;

namespace FiveLib.Ausar.Tags
{
    /// <summary>
    /// A Blam string_id value.
    /// </summary>
    public struct StringId : IEquatable<StringId>, IComparable<StringId>
    {
        /// <summary>
        /// Constructs a <see cref="StringId"/> from its integer value.
        /// </summary>
        /// <param name="val">The integer value.</param>
        public StringId(uint val)
        {
            Value = val;
        }

        /// <summary>
        /// Constructs a <see cref="StringId"/> by hashing a string value.
        /// </summary>
        /// <param name="str">The string value.</param>
        public StringId(string str)
        {
            // string_id values are just Murmur3_32 hashes
            var murmur32 = MurmurHash.Create32(managed: false);
            var hash = murmur32.ComputeHash(Encoding.UTF8.GetBytes(str));
            Value = (uint)((hash[3] << 24) | (hash[2] << 16) | (hash[1] << 8) | hash[0]);
        }

        /// <summary>
        /// The integer value of the string_id.
        /// </summary>
        public readonly uint Value;

        public bool Equals(StringId other) => Value == other.Value;

        public override bool Equals(object obj)
        {
            if (!(obj is StringId))
                return false;
            return Equals((StringId)obj);
        }

        public static bool operator ==(StringId x, StringId y) => x.Equals(y);

        public static bool operator !=(StringId x, StringId y) => !(x == y);

        public int CompareTo(StringId other) => Value.CompareTo(other.Value);

        public override string ToString() => $"0x{Value:X8}";

        public override int GetHashCode() => (int)Value;
    }
}

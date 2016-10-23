using System;
using System.IO;
using FiveLib.Ausar.Memory.Stl;
using FiveLib.Common;
using FiveLib.IO;

namespace FiveLib.Ausar.Memory.Tags
{
    /// <summary>
    /// A 32-bit ID value which can be hashed with FNV-1a.
    /// </summary>
    public class HashableId32 : IBinarySerializable, IFixedSize
    {
        public HashableId32()
        {
        }

        public HashableId32(uint value)
        {
            Value = value;
        }

        public uint Value;

        public void Serialize(BinarySerializer s)
        {
            s.Value(ref Value);
        }

        public ulong GetStructSize() => 4;

        public ulong GetStructAlignment() => 4;

        public override bool Equals(object obj)
        {
            var other = obj as HashableId32;
            return Value == other?.Value;
        }

        public override int GetHashCode()
        {
            return (int)new Fnv1A64Hasher().Hash(this);
        }

        public override string ToString() => $"0x{Value:X}";

        public class Fnv1A64Hasher : IStlHash<HashableId32>
        {
            public ulong Hash(HashableId32 id)
            {
                var idBytes = BitConverter.GetBytes(id.Value);
                return Fnv1A64Hash.ComputeValue(idBytes);
            }
        }
    }
}

using System.IO;
using FiveLib.IO;

namespace FiveLib.Memory
{
    /// <summary>
    /// A 64-bit memory address which points to serializable binary data.
    /// </summary>
    public struct Pointer64<T> : IFixedSize
        where T: IBinarySerializable, IFixedSize, new()
    {
        public static readonly Pointer64<T> Null = new Pointer64<T>(0); 

        public Pointer64(ulong address)
        {
            Address = address;
        }

        public readonly ulong Address;

        public T Get(BinaryReader reader)
        {
            var result = new T();
            reader.BaseStream.Position = (long)Address;
            result.Serialize(new BinarySerializer(reader));
            return result;
        }

        public T Get(ulong index, BinaryReader reader)
        {
            var result = new T();
            reader.BaseStream.Position = (long)(Address + index * result.GetStructSize());
            result.Serialize(new BinarySerializer(reader));
            return result;
        }

        public void Set(T newValue, BinaryWriter writer)
        {
            writer.BaseStream.Position = (long)Address;
            newValue.Serialize(new BinarySerializer(writer));
        }

        public void Set(ulong index, T newValue, BinaryWriter writer)
        {
            writer.BaseStream.Position = (long)(Address + index * newValue.GetStructSize());
            newValue.Serialize(new BinarySerializer(writer));
        }

        public static Pointer64<T> Read(BinaryReader reader)
        {
            return new Pointer64<T>(reader.ReadUInt64());
        }

        public ulong GetStructSize() => 8;

        public ulong GetStructAlignment() => 8;

        public override bool Equals(object obj)
        {
            if (!(obj is Pointer64<T>))
                return false;
            var other = (Pointer64<T>)obj;
            return this == other;
        }

        public static bool operator ==(Pointer64<T> lhs, Pointer64<T> rhs)
        {
            return lhs.Address == rhs.Address;
        }

        public static bool operator !=(Pointer64<T> lhs, Pointer64<T> rhs)
        {
            return !(lhs == rhs);
        }

        public override int GetHashCode()
        {
            return Address.GetHashCode();
        }

        public override string ToString() => $"({typeof(T)}*)0x{Address:X}";

        /// <summary>
        /// Wraps a <see cref="Pointer64{T}"/> so that it is mutable.
        /// Useful for pointers which point to other pointers.
        /// </summary>
        public class Wrapper : IBinarySerializable, IFixedSize
        {
            public Wrapper()
            {
                Pointer = Null;
            }

            public Wrapper(Pointer64<T> pointer)
            {
                Pointer = pointer;
            }

            public Pointer64<T> Pointer;

            public ulong GetStructSize() => Pointer.GetStructSize();

            public ulong GetStructAlignment() => Pointer.GetStructAlignment();

            public override string ToString() => Pointer.ToString();

            public void Serialize(BinarySerializer s)
            {
                s.Value(ref Pointer);
            }
        }
    }
}

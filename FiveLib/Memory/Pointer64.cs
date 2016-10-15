using System.IO;
using FiveLib.Common;

namespace FiveLib.Memory
{
    /// <summary>
    /// A 64-bit memory address which points to serializable binary data.
    /// </summary>
    /// <seealso cref="FiveLib.Common.IBinaryWritable" />
    /// <seealso cref="FiveLib.Common.IBinaryStruct" />
    public struct Pointer64<T> : IBinaryWritable, IBinaryStruct
        where T: IBinaryReadable, IBinaryWritable, IBinaryStruct, new()
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
            result.Read(reader);
            return result;
        }

        public T Get(ulong index, BinaryReader reader)
        {
            var result = new T();
            reader.BaseStream.Position = (long)(Address + index * result.GetStructSize());
            result.Read(reader);
            return result;
        }

        public void Set(T newValue, BinaryWriter writer)
        {
            writer.BaseStream.Position = (long)Address;
            newValue.Write(writer);
        }

        public void Set(ulong index, T newValue, BinaryWriter writer)
        {
            writer.BaseStream.Position = (long)(Address + index * newValue.GetStructSize());
            newValue.Write(writer);
        }

        public static Pointer64<T> Read(BinaryReader reader)
        {
            return new Pointer64<T>(reader.ReadUInt64());
        } 

        public void Write(BinaryWriter writer)
        {
            writer.Write(Address);
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
        public class Wrapper : IBinaryReadable, IBinaryWritable, IBinaryStruct
        {
            public Wrapper()
            {
                Pointer = Null;
            }

            public Wrapper(Pointer64<T> pointer)
            {
                Pointer = pointer;
            }

            public Pointer64<T> Pointer { get; set; }

            public void Read(BinaryReader reader)
            {
                Pointer = Pointer64<T>.Read(reader);
            }

            public void Write(BinaryWriter writer)
            {
                Pointer.Write(writer);
            }

            public ulong GetStructSize() => Pointer.GetStructSize();

            public ulong GetStructAlignment() => Pointer.GetStructAlignment();

            public override string ToString() => Pointer.ToString();
        }
    }
}

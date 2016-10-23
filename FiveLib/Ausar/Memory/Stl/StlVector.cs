using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FiveLib.Common;
using FiveLib.IO;
using FiveLib.Memory;

namespace FiveLib.Ausar.Memory.Stl
{
    /// <summary>
    /// Memory interface for std::vector.
    /// </summary>
    public class StlVector<T> : IBinarySerializable, IFixedSize
        where T: IBinarySerializable, IFixedSize, new()
    {
        private readonly ulong _elementSize;

        public StlVector()
        {
            _elementSize = new T().GetStructSize(); // TODO: is there any way to make this not suck?
        }

        public Pointer64<T> First = Pointer64<T>.Null;
        public Pointer64<T> Last = Pointer64<T>.Null;
        public Pointer64<T> End = Pointer64<T>.Null;

        public T Get(ulong index, BinaryReader reader)
        {
            if (index >= Size)
                throw new IndexOutOfRangeException();
            return First.Get(index, reader);
        }

        public void Set(ulong index, T newValue, BinaryWriter writer)
        {
            if (index >= Size)
                throw new IndexOutOfRangeException();
            First.Set(index, newValue, writer);
        }

        public ulong Size => (Last.Address - First.Address) / _elementSize;

        public ulong Capacity => (End.Address - First.Address) / _elementSize;

        public void Serialize(BinarySerializer s)
        {
            s.Value(ref First);
            s.Value(ref Last);
            s.Value(ref End);
        }

        public ulong GetStructSize() => 0xC;

        public ulong GetStructAlignment() => 0x8;
    }
}

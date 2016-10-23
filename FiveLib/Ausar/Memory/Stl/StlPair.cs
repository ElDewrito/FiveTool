using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FiveLib.Common;
using FiveLib.IO;

namespace FiveLib.Ausar.Memory.Stl
{
    /// <summary>
    /// Memory interface for std::pair.
    /// </summary>
    public class StlPair<TFirst, TSecond> : IBinarySerializable, IFixedSize
        where TFirst: IBinarySerializable, IFixedSize, new()
        where TSecond: IBinarySerializable, IFixedSize, new()
    {
        public StlPair()
        {
            First = new TFirst();
            Second = new TSecond();
        }

        public StlPair(TFirst first, TSecond second)
        {
            First = first;
            Second = second;
        }

        public TFirst First;
        public TSecond Second;

        public void Serialize(BinarySerializer s)
        {
            s.Object(ref First);
            s.Padding(GetPadding(First.GetStructSize()));
            s.Object(ref Second);
            s.Padding(GetPadding(Second.GetStructSize()));
        }

        public ulong GetStructSize()
        {
            return AlignSize(First.GetStructSize()) + AlignSize(Second.GetStructSize());
        }

        public ulong GetStructAlignment() => Math.Max(First.GetStructAlignment(), Second.GetStructAlignment());

        private ulong AlignSize(ulong size)
        {
            return AlignmentUtil.Align(size, GetStructAlignment());
        }

        private int GetPadding(ulong size)
        {
            return (int)(AlignSize(size) - size);
        }
    }
}

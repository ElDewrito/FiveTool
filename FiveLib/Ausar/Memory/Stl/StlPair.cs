using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FiveLib.Common;

namespace FiveLib.Ausar.Memory.Stl
{
    /// <summary>
    /// Memory interface for std::pair.
    /// </summary>
    public class StlPair<TFirst, TSecond> : IBinaryReadable, IBinaryWritable, IBinaryStruct
        where TFirst: IBinaryReadable, IBinaryWritable, IBinaryStruct, new()
        where TSecond: IBinaryReadable, IBinaryWritable, IBinaryStruct, new()
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
         
        public TFirst First { get; set; }

        public TSecond Second { get; set; }

        public void Read(BinaryReader reader)
        {
            First = new TFirst();
            Second = new TSecond();
            First.Read(reader);
            reader.BaseStream.Position += GetPadding(First.GetStructSize());
            Second.Read(reader);
            reader.BaseStream.Position += GetPadding(Second.GetStructSize());
        }

        public void Write(BinaryWriter writer)
        {
            First.Write(writer);
            writer.BaseStream.Position += GetPadding(First.GetStructSize());
            Second.Write(writer);
            writer.BaseStream.Position += GetPadding(Second.GetStructSize());
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

        private long GetPadding(ulong size)
        {
            return (long)(AlignSize(size) - size);
        }
    }
}

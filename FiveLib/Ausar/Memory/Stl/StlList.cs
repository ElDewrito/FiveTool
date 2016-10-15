using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FiveLib.Common;
using FiveLib.Memory;

namespace FiveLib.Ausar.Memory.Stl
{
    /// <summary>
    /// Memory interface for std::list.
    /// </summary>
    public class StlList<T> : IBinaryReadable, IBinaryWritable, IBinaryStruct
        where T: IBinaryReadable, IBinaryWritable, IBinaryStruct, new()
    {
        public Pointer64<Node> End { get; set; } = Pointer64<Node>.Null; 

        public ulong Count { get; set; }

        public Pointer64<Node> GetBegin(BinaryReader reader)
        {
            return End.Get(reader).Next;
        }

        public IEnumerable<Node> Enumerate(BinaryReader reader)
        {
            var currentNode = GetBegin(reader);
            while (currentNode != End)
            {
                var nodeData = currentNode.Get(reader);
                yield return nodeData;
                currentNode = nodeData.Next;
            }
        } 

        public void Read(BinaryReader reader)
        {
            End = Pointer64<Node>.Read(reader);
            Count = reader.ReadUInt64();
        }

        public void Write(BinaryWriter writer)
        {
            End.Write(writer);
            writer.Write(Count);
        }

        public ulong GetStructSize() => 0x10;

        public ulong GetStructAlignment() => 0x8;

        public class Node : IBinaryReadable, IBinaryWritable, IBinaryStruct
        {
            public Pointer64<Node> Next { get; set; } = Pointer64<Node>.Null;

            public Pointer64<Node> Previous { get; set; } = Pointer64<Node>.Null;

            public T Data { get; set; }

            public void Read(BinaryReader reader)
            {
                Next = Pointer64<Node>.Read(reader);
                Previous = Pointer64<Node>.Read(reader);
                Data = new T();
                Data.Read(reader);
            }

            public void Write(BinaryWriter writer)
            {
                Next.Write(writer);
                Previous.Write(writer);
                Data.Write(writer);
            }

            public ulong GetStructSize() => 0x10 + Data.GetStructSize();

            public ulong GetStructAlignment() => Math.Max(8, Data.GetStructAlignment());
        }
    }
}

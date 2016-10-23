using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FiveLib.IO;
using FiveLib.Memory;

namespace FiveLib.Ausar.Memory.Stl
{
    /// <summary>
    /// Memory interface for std::list.
    /// </summary>
    public class StlList<T> : IBinarySerializable, IFixedSize
        where T: IBinarySerializable, IFixedSize, new()
    {
        public Pointer64<Node> End = Pointer64<Node>.Null;
        public ulong Count;

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

        public void Serialize(BinarySerializer s)
        {
            s.Value(ref End);
            s.Value(ref Count);
        }

        public ulong GetStructSize() => 0x10;

        public ulong GetStructAlignment() => 0x8;

        public class Node : IBinarySerializable, IFixedSize
        {
            public Pointer64<Node> Next = Pointer64<Node>.Null;
            public Pointer64<Node> Previous = Pointer64<Node>.Null;
            public T Data;

            public void Serialize(BinarySerializer s)
            {
                s.Value(ref Next);
                s.Value(ref Previous);
                s.Object(ref Data);
            }

            public ulong GetStructSize() => 0x10 + Data.GetStructSize();

            public ulong GetStructAlignment() => Math.Max(8, Data.GetStructAlignment());
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FiveLib.Memory;

namespace FiveLib.Ausar.Memory.Tags
{
    public class MemoryTagAddressList
    {
        private const int MaxTagCount = 0x11800;

        private readonly ulong _address;

        public MemoryTagAddressList(ulong address)
        {
            _address = address;
        }

        public bool TryGetTagAddress(uint localHandle, BinaryReader reader, out ulong address)
        {
            var index = localHandle >> 15;
            if (index >= MaxTagCount)
            {
                address = 0;
                return false;
            }
            reader.BaseStream.Position = (long)(_address + index * 8);
            address = reader.ReadUInt64();
            return address != 0;
        }
    }
}

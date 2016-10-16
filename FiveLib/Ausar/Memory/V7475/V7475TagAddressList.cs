using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FiveLib.Ausar.Memory.Tags;

namespace FiveLib.Ausar.Memory.V7475
{
    public static class V7475TagAddressList
    {
        private const long AddressListPointerRva = 0x73520B8;

        public static MemoryTagAddressList Read(Process process, BinaryReader reader)
        {
            reader.BaseStream.Position = (long)process.MainModule.BaseAddress + AddressListPointerRva;
            var address = reader.ReadUInt64();
            if (address == 0)
                throw new InvalidOperationException("No tag address list is available");
            return new MemoryTagAddressList(address);
        }
    }
}

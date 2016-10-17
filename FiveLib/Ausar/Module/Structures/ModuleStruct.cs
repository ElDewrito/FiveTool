using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FiveLib.Common;

namespace FiveLib.Ausar.Module.Structures
{
    internal class ModuleStruct
    {
        public ModuleFileHeaderStruct FileHeader { get; set; } = new ModuleFileHeaderStruct();

        public ModuleEntryStruct[] Entries { get; set; } = new ModuleEntryStruct[0];

        public StringBlob StringTable { get; set; } = new StringBlob(Encoding.UTF8);

        public int[] ResourceEntries { get; set; } = new int[0];

        public ModuleDataBlockStruct[] CompressedBlocks { get; set; } = new ModuleDataBlockStruct[0];

        public void Read(BinaryReader reader)
        {
            FileHeader = ReadAndValidateFileHeader(reader);
            Entries = ReadFileEntries(reader, FileHeader);
            StringTable = ReadStringTable(reader, FileHeader);
            ResourceEntries = ReadResourceEntries(reader, FileHeader);
            CompressedBlocks = ReadCompressedBlocks(reader, FileHeader);
        }

        private static ModuleFileHeaderStruct ReadAndValidateFileHeader(BinaryReader reader)
        {
            var header = new ModuleFileHeaderStruct();
            header.Read(reader);
            ValidateHeader(header);
            return header;
        }

        private static void ValidateHeader(ModuleFileHeaderStruct header)
        {
            if (header.Magic != ModuleFileHeaderStruct.ExpectedMagic)
                throw new InvalidDataException($"Invalid module magic {header.Magic}");
            if (header.Version != ModuleFileHeaderStruct.ExpectedVersion)
                throw new InvalidDataException($"Unsupported module version {header.Version}");
        }

        private static ModuleEntryStruct[] ReadFileEntries(BinaryReader reader, ModuleFileHeaderStruct header)
        {
            var entries = new ModuleEntryStruct[header.FileCount];
            for (var i = 0; i < header.FileCount; i++)
            {
                var entry = new ModuleEntryStruct();
                entry.Read(reader);
                entries[i] = entry;
            }
            return entries;
        }

        private static StringBlob ReadStringTable(BinaryReader reader, ModuleFileHeaderStruct header)
        {
            var bytes = reader.ReadBytes((int)header.StringTableSize);
            return new StringBlob(Encoding.UTF8, bytes);
        }

        private static int[] ReadResourceEntries(BinaryReader reader, ModuleFileHeaderStruct header)
        {
            var files = new int[header.ResourceCount];
            for (var i = 0; i < header.ResourceCount; i++)
                files[i] = reader.ReadInt32();
            return files;
        }

        private static ModuleDataBlockStruct[] ReadCompressedBlocks(BinaryReader reader, ModuleFileHeaderStruct header)
        {
            var blocks = new ModuleDataBlockStruct[header.DataBlockCount];
            for (var i = 0; i < header.DataBlockCount; i++)
            {
                var block = new ModuleDataBlockStruct();
                block.Read(reader);
                blocks[i] = block;
            }
            return blocks;
        }
    }
}

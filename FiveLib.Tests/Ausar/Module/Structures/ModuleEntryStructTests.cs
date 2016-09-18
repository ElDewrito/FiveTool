using System;
using System.IO;
using FiveLib.Ausar.Module.Structures;
using FiveLib.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FiveLib.Tests.Ausar.Module.Structures
{
    [TestClass]
    public class ModuleEntryStructTests
    {
        private static readonly byte[] DummyEntryBytes =
        {
            0x01, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00, 0x03, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00,
            0x05, 0x00, 0x00, 0x00, 0x06, 0x00, 0x00, 0x00, 0x07, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x08, 0x00, 0x00, 0x00, 0x09, 0x00, 0x00, 0x00, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x00, 0x00, 0x00,
            0x0F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x10, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x64, 0x63, 0x62, 0x61, 0x11, 0x00, 0x00, 0x00, 0x12, 0x00, 0x00, 0x00, 0x13, 0x00, 0x00, 0x00,
            0x14, 0x00, 0x15, 0x00, 0x16, 0x00, 0x00, 0x00
        };

        [TestMethod]
        public void TestReadingDummyEntry()
        {
            var entry = new ModuleEntryStruct();
            using (var reader = new BinaryReader(new MemoryStream(DummyEntryBytes)))
                entry.Read(reader);

            Assert.AreEqual(1U, entry.NameOffset);
            Assert.AreEqual(2, entry.ParentFileIndex);
            Assert.AreEqual(3, entry.Unknown8);
            Assert.AreEqual(4, entry.UnknownC);
            Assert.AreEqual(5, entry.CompressedBlockCount);
            Assert.AreEqual(6, entry.FirstCompressedBlockIndex);
            Assert.AreEqual(7U, entry.CompressedOffset);
            Assert.AreEqual(8U, entry.TotalCompressedSize);
            Assert.AreEqual(9U, entry.TotalUncompressedSize);
            Assert.AreEqual(10, entry.Unknown28);
            Assert.AreEqual(11, entry.Unknown29);
            Assert.AreEqual(12, entry.Unknown2A);
            Assert.AreEqual(13, entry.Unknown2B);
            Assert.AreEqual(14, entry.GlobalTagId);
            Assert.AreEqual(15, entry.SourceTagId);
            Assert.AreEqual(16, entry.Unknown38);
            Assert.AreEqual(new MagicNumber("abcd"), entry.GroupTag);
            Assert.AreEqual(17U, entry.UncompressedHeaderSize);
            Assert.AreEqual(18U, entry.UncompressedTagDataSize);
            Assert.AreEqual(19U, entry.UncompressedResourceDataSize);
            Assert.AreEqual(20, entry.Unknown50);
            Assert.AreEqual(21, entry.Unknown52);
            Assert.AreEqual(22, entry.Unknown54);
        }

        [TestMethod]
        public void TestWritingDummyEntry()
        {
            var entry = new ModuleEntryStruct
            {
                NameOffset = 1,
                ParentFileIndex = 2,
                Unknown8 = 3,
                UnknownC = 4,
                CompressedBlockCount = 5,
                FirstCompressedBlockIndex = 6,
                CompressedOffset = 7,
                TotalCompressedSize = 8,
                TotalUncompressedSize = 9,
                Unknown28 = 10,
                Unknown29 = 11,
                Unknown2A = 12,
                Unknown2B = 13,
                GlobalTagId = 14,
                SourceTagId = 15,
                Unknown38 = 16,
                GroupTag = new MagicNumber("abcd"),
                UncompressedHeaderSize = 17,
                UncompressedTagDataSize = 18,
                UncompressedResourceDataSize = 19,
                Unknown50 = 20,
                Unknown52 = 21,
                Unknown54 = 22
            };

            byte[] writtenBytes;
            using (var stream = new MemoryStream())
            {
                using (var writer = new BinaryWriter(stream))
                {
                    entry.Write(writer);
                    writtenBytes = new byte[stream.Length];
                    Buffer.BlockCopy(stream.GetBuffer(), 0, writtenBytes, 0, writtenBytes.Length);
                }
            }
            CollectionAssert.AreEqual(DummyEntryBytes, writtenBytes);
        }
    }
}

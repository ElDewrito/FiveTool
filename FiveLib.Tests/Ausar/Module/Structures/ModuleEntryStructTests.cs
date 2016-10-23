using System;
using System.IO;
using FiveLib.Ausar.Module.Structures;
using FiveLib.Common;
using FiveLib.IO;
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
            using (var serializer = new BinarySerializer(new BinaryReader(new MemoryStream(DummyEntryBytes))))
                entry.Serialize(serializer);

            Assert.AreEqual(1U, entry.NameOffset);
            Assert.AreEqual(2, entry.ParentFileIndex);
            Assert.AreEqual(3, entry.ResourceCount);
            Assert.AreEqual(4, entry.FirstResourceIndex);
            Assert.AreEqual(5, entry.BlockCount);
            Assert.AreEqual(6, entry.FirstBlockIndex);
            Assert.AreEqual(7, entry.DataOffset);
            Assert.AreEqual(8U, entry.TotalCompressedSize);
            Assert.AreEqual(9U, entry.TotalUncompressedSize);
            Assert.AreEqual(10, entry.HeaderAlignment);
            Assert.AreEqual(11, entry.TagDataAlignment);
            Assert.AreEqual(12, entry.ResourceDataAlignment);
            Assert.AreEqual((ModuleEntryFlags)13, entry.Flags);
            Assert.AreEqual(14U, entry.GlobalId);
            Assert.AreEqual(15U, entry.AssetId);
            Assert.AreEqual(16U, entry.AssetChecksum);
            Assert.AreEqual(new MagicNumber("abcd"), entry.GroupTag);
            Assert.AreEqual(17U, entry.UncompressedHeaderSize);
            Assert.AreEqual(18U, entry.UncompressedTagDataSize);
            Assert.AreEqual(19U, entry.UncompressedResourceDataSize);
            Assert.AreEqual(20, entry.HeaderBlockCount);
            Assert.AreEqual(21, entry.TagDataBlockCount);
            Assert.AreEqual(22, entry.ResourceBlockCount);
        }

        [TestMethod]
        public void TestWritingDummyEntry()
        {
            var entry = new ModuleEntryStruct
            {
                NameOffset = 1,
                ParentFileIndex = 2,
                ResourceCount = 3,
                FirstResourceIndex = 4,
                BlockCount = 5,
                FirstBlockIndex = 6,
                DataOffset = 7,
                TotalCompressedSize = 8,
                TotalUncompressedSize = 9,
                HeaderAlignment = 10,
                TagDataAlignment = 11,
                ResourceDataAlignment = 12,
                Flags = (ModuleEntryFlags)13,
                GlobalId = 14,
                AssetId = 15,
                AssetChecksum = 16,
                GroupTag = new MagicNumber("abcd"),
                UncompressedHeaderSize = 17,
                UncompressedTagDataSize = 18,
                UncompressedResourceDataSize = 19,
                HeaderBlockCount = 20,
                TagDataBlockCount = 21,
                ResourceBlockCount = 22
            };

            byte[] writtenBytes;
            using (var stream = new MemoryStream())
            {
                using (var serializer = new BinarySerializer(new BinaryWriter(stream)))
                {
                    entry.Serialize(serializer);
                    writtenBytes = new byte[stream.Length];
                    Buffer.BlockCopy(stream.GetBuffer(), 0, writtenBytes, 0, writtenBytes.Length);
                }
            }
            CollectionAssert.AreEqual(DummyEntryBytes, writtenBytes);
        }
    }
}

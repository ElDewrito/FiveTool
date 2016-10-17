using System;
using System.IO;
using FiveLib.Ausar.Module.Structures;
using FiveLib.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FiveLib.Tests.Ausar.Module.Structures
{
    [TestClass]
    public class ModuleFileHeaderStructTests
    {
        private static readonly byte[] DummyHeaderBytes =
        {
            0x6D, 0x6F, 0x68, 0x64, 0x1B, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x02, 0x00, 0x00, 0x00, 0x03, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00, 0x05, 0x00, 0x00, 0x00,
            0x06, 0x00, 0x00, 0x00, 0x07, 0x00, 0x00, 0x00, 0x08, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x09, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
        };

        [TestMethod]
        public void TestReadingDummyHeader()
        {
            var header = new ModuleFileHeaderStruct();
            using (var reader = new BinaryReader(new MemoryStream(DummyHeaderBytes)))
            {
                header.Read(reader);
                Assert.AreEqual(DummyHeaderBytes.Length, reader.BaseStream.Position);
            }

            Assert.AreEqual(new MagicNumber("dhom"), header.Magic);
            Assert.AreEqual(27, header.Version);
            Assert.AreEqual(1UL, header.Id);
            Assert.AreEqual(2, header.FileCount);
            Assert.AreEqual(3, header.LoadedTagCount);
            Assert.AreEqual(4, header.FirstResourceIndex);
            Assert.AreEqual(5U, header.StringTableSize);
            Assert.AreEqual(6, header.ResourceCount);
            Assert.AreEqual(7, header.DataBlockCount);
            Assert.AreEqual(8U, header.BuildVersionId);
            Assert.AreEqual(9U, header.HeaderChecksum);
        }

        [TestMethod]
        public void TestWritingDummyHeader()
        {
            var header = new ModuleFileHeaderStruct
            {
                Magic = new MagicNumber("dhom"),
                Version = 27,
                Id = 1,
                FileCount = 2,
                LoadedTagCount = 3,
                FirstResourceIndex = 4,
                StringTableSize = 5,
                ResourceCount = 6,
                DataBlockCount = 7,
                BuildVersionId = 8,
                HeaderChecksum = 9,
            };

            byte[] writtenBytes;
            using (var stream = new MemoryStream())
            {
                using (var writer = new BinaryWriter(stream))
                {
                    header.Write(writer);
                    writtenBytes = new byte[stream.Length];
                    Buffer.BlockCopy(stream.GetBuffer(), 0, writtenBytes, 0, writtenBytes.Length);
                }
            }
            CollectionAssert.AreEqual(DummyHeaderBytes, writtenBytes);
        }
    }
}

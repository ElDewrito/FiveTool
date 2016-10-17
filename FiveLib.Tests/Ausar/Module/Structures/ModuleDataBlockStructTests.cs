using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FiveLib.Ausar.Module.Structures;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FiveLib.Tests.Ausar.Module.Structures
{
    [TestClass]
    public class ModuleDataBlockStructTests
    {
        private static readonly byte[] DummyBlockBytes =
        {
            0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00, 0x03, 0x00, 0x00, 0x00,
            0x04, 0x00, 0x00, 0x00, 0x05, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
        };

        [TestMethod]
        public void TestReadingDummyCompressedBlock()
        {
            var block = new ModuleDataBlockStruct();
            using (var reader = new BinaryReader(new MemoryStream(DummyBlockBytes)))
            {
                block.Read(reader);
                Assert.AreEqual(DummyBlockBytes.Length, reader.BaseStream.Position);
            }

            Assert.AreEqual(1U, block.Checksum);
            Assert.AreEqual(2U, block.CompressedOffset);
            Assert.AreEqual(3U, block.CompressedSize);
            Assert.AreEqual(4U, block.UncompressedOffset);
            Assert.AreEqual(5U, block.UncompressedSize);
            Assert.AreEqual(true, block.IsCompressed);
        }

        [TestMethod]
        public void TestWritingDummyCompressedBlock()
        {
            var block = new ModuleDataBlockStruct
            {
                Checksum = 1,
                CompressedOffset = 2,
                CompressedSize = 3,
                UncompressedOffset = 4,
                UncompressedSize = 5,
                IsCompressed = true,
            };

            byte[] writtenBytes;
            using (var stream = new MemoryStream())
            {
                using (var writer = new BinaryWriter(stream))
                {
                    block.Write(writer);
                    writtenBytes = new byte[stream.Length];
                    Buffer.BlockCopy(stream.GetBuffer(), 0, writtenBytes, 0, writtenBytes.Length);
                }
            }
            CollectionAssert.AreEqual(DummyBlockBytes, writtenBytes);
        }
    }
}

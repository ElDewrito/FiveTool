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
    public class ModuleCompressedBlockStructTests
    {
        private static readonly byte[] DummyBlockBytes =
        {
            0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00, 0x03, 0x00, 0x00, 0x00,
            0x04, 0x00, 0x00, 0x00, 0x05, 0x00, 0x00, 0x00, 0x06, 0x00, 0x00, 0x00, 0x07, 0x00, 0x00, 0x00,
        };

        [TestMethod]
        public void TestReadingDummyCompressedBlock()
        {
            var block = new ModuleCompressedBlockStruct();
            using (var reader = new BinaryReader(new MemoryStream(DummyBlockBytes)))
                block.Read(reader);

            Assert.AreEqual(1, block.Unknown0);
            Assert.AreEqual(2U, block.CompressedOffset);
            Assert.AreEqual(3U, block.CompressedSize);
            Assert.AreEqual(4U, block.UncompressedOffset);
            Assert.AreEqual(5U, block.UncompressedSize);
            Assert.AreEqual(6, block.Unknown18);
            Assert.AreEqual(7, block.Unknown1C);
        }

        [TestMethod]
        public void TestWritingDummyCompressedBlock()
        {
            var block = new ModuleCompressedBlockStruct
            {
                Unknown0 = 1,
                CompressedOffset = 2,
                CompressedSize = 3,
                UncompressedOffset = 4,
                UncompressedSize = 5,
                Unknown18 = 6,
                Unknown1C = 7,
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

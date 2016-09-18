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
            0x6D, 0x6F, 0x68, 0x64, 0x1B, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00,
            0x03, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00, 0x05, 0x00, 0x00, 0x00, 0x06, 0x00, 0x00, 0x00,
            0x07, 0x00, 0x00, 0x00, 0x08, 0x00, 0x00, 0x00, 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07,
            0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F
        };

        [TestMethod]
        public void TestReadingDummyHeader()
        {
            var header = new ModuleFileHeaderStruct();
            using (var reader = new BinaryReader(new MemoryStream(DummyHeaderBytes)))
                header.Read(reader);

            Assert.AreEqual(new MagicNumber("dhom"), header.Magic);
            Assert.AreEqual(27, header.Version);
            Assert.AreEqual(1, header.Unknown8);
            Assert.AreEqual(2, header.UnknownC);
            Assert.AreEqual(3, header.FileCount);
            Assert.AreEqual(4, header.Unknown14);
            Assert.AreEqual(5, header.Unknown18);
            Assert.AreEqual(6U, header.StringTableSize);
            Assert.AreEqual(7, header.ResourceCount);
            Assert.AreEqual(8, header.CompressedBlockCount);

            var dummyUnknown28 = new byte[0x10];
            for (var i = 0; i < 0x10; i++)
                dummyUnknown28[i] = (byte)i;
            CollectionAssert.AreEqual(dummyUnknown28, header.Unknown28);
        }

        [TestMethod]
        public void TestWritingDummyHeader()
        {
            var header = new ModuleFileHeaderStruct
            {
                Magic = new MagicNumber("dhom"),
                Version = 27,
                Unknown8 = 1,
                UnknownC = 2,
                FileCount = 3,
                Unknown14 = 4,
                Unknown18 = 5,
                StringTableSize = 6,
                ResourceCount = 7,
                CompressedBlockCount = 8,
            };
            for (var i = 0; i < header.Unknown28.Length; i++)
                header.Unknown28[i] = (byte)i;

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

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FiveLib.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FiveLib.Tests.Common
{
    [TestClass]
    public class StringBlobTests
    {
        [TestMethod]
        public void TestConstructingEmptyBlob()
        {
            var blob = new StringBlob(Encoding.UTF8);
            Assert.AreEqual(0, blob.Length);
        }

        [TestMethod]
        public void TestConstructingInitializedBlob()
        {
            var bytes = new byte[] { 0x66, 0x6F, 0x6F, 0x00 };
            var blob = new StringBlob(Encoding.UTF8, bytes);
            Assert.AreEqual(4, blob.Length);
        }

        [TestMethod]
        public void TestGetBytes()
        {
            var bytes = new byte[] { 0x66, 0x6F, 0x6F, 0x00 };
            var blob = new StringBlob(Encoding.UTF8, bytes);
            var bytes2 = blob.GetBytes();
            Assert.AreNotSame(bytes, bytes2);
            CollectionAssert.AreEqual(bytes, bytes2);
        }

        [TestMethod]
        public void TestWriteTo()
        {
            var bytes = new byte[] { 0x66, 0x6F, 0x6F, 0x00 };
            var blob = new StringBlob(Encoding.UTF8, bytes);
            using (var stream = new MemoryStream())
            {
                blob.WriteTo(stream);
                Assert.AreEqual(blob.Length, stream.Length);

                var bytes2 = new byte[stream.Length];
                Buffer.BlockCopy(stream.GetBuffer(), 0, bytes2, 0, (int)stream.Length);
                CollectionAssert.AreEqual(bytes, bytes2);
            }
        }

        [TestMethod]
        public void TestGetStringAtOffsetForAscii()
        {
            var bytes = new byte[]
            {
                0x66, 0x6F, 0x6F, 0x00,
                0x62, 0x61, 0x72, 0x00,
            };
            var blob = new StringBlob(Encoding.ASCII, bytes);

            Assert.AreEqual("foo", blob.GetStringAtOffset(0));
            Assert.AreEqual("bar", blob.GetStringAtOffset(4));
            Assert.AreEqual("", blob.GetStringAtOffset(3));
        }

        [TestMethod]
        public void TestGetStringAtOffsetForUtf8()
        {
            var bytes = new byte[]
            {
                0x66, 0x6F, 0x6F, 0x00,
                0x50, 0x6F, 0x6F, 0x70, 0x20, 0xF0, 0x9F, 0x92, 0xA9, 0x21, 0x00,
                0x62, 0x61, 0x72, 0x00,
            };
            var blob = new StringBlob(Encoding.UTF8, bytes);

            Assert.AreEqual("foo", blob.GetStringAtOffset(0));
            Assert.AreEqual("Poop 💩!", blob.GetStringAtOffset(4));
            Assert.AreEqual("bar", blob.GetStringAtOffset(15));
        }

        [TestMethod]
        public void TestGetStringAtOffsetForUtf16()
        {
            var bytes = new byte[]
            {
                0x66, 0x00, 0x6F, 0x00, 0x6F, 0x00, 0x00, 0x00,
                0x50, 0x00, 0x6F, 0x00, 0x6F, 0x00, 0x70, 0x00, 0x20, 0x00, 0x3D, 0xD8, 0xA9, 0xDC, 0x21, 0x00, 0x00, 0x00,
                0x62, 0x00, 0x61, 0x00, 0x72, 0x00, 0x00, 0x00,
            };
            var blob = new StringBlob(Encoding.Unicode, bytes);

            Assert.AreEqual("foo", blob.GetStringAtOffset(0));
            Assert.AreEqual("Poop 💩!", blob.GetStringAtOffset(8));
            Assert.AreEqual("bar", blob.GetStringAtOffset(26));
        }

        [TestMethod]
        public void TestGetStringAtOffsetOutOfRange()
        {
            var bytes = new byte[]
            {
                0x66, 0x6F, 0x6F, 0x00,
                0x62, 0x61, 0x72, 0x00,
            };
            var blob = new StringBlob(Encoding.ASCII, bytes);

            try
            {
                blob.GetStringAtOffset(-1);
                Assert.Fail("Negative offset did not throw an ArgumentOutOfRangeException");
            }
            catch (ArgumentOutOfRangeException)
            {
            }

            try
            {
                blob.GetStringAtOffset(8);
                Assert.Fail("Large offset did not throw an ArgumentOutOfRangeException");
            }
            catch (ArgumentOutOfRangeException)
            {
            }
        }

        [TestMethod]
        public void TestGetStringAtOffsetWithoutNullTerminator()
        {
            var bytes = new byte[] { 0x66, 0x6F, 0x6F };
            var blob = new StringBlob(Encoding.ASCII, bytes);
            Assert.AreEqual("foo", blob.GetStringAtOffset(0));
        }

        [TestMethod]
        public void TestAddStringToEmptyBlob()
        {
            var blob = new StringBlob(Encoding.ASCII);
            var offset1 = blob.AddString("foo");
            var offset2 = blob.AddString("bar");
            Assert.AreEqual(0, offset1);
            Assert.AreEqual(4, offset2);
            Assert.AreEqual(8, blob.Length);

            var expected = new byte[]
            {
                0x66, 0x6F, 0x6F, 0x00,
                0x62, 0x61, 0x72, 0x00,
            };
            CollectionAssert.AreEqual(expected, blob.GetBytes());
        }

        [TestMethod]
        public void TestAddStringToInitializedBlob()
        {
            var bytes = new byte[] { 0x66, 0x6F, 0x6F, 0x00 };
            var blob = new StringBlob(Encoding.ASCII, bytes);
            var offset = blob.AddString("bar");
            Assert.AreEqual(4, offset);
            Assert.AreEqual(8, blob.Length);

            var expected = new byte[]
            {
                0x66, 0x6F, 0x6F, 0x00,
                0x62, 0x61, 0x72, 0x00,
            };
            CollectionAssert.AreEqual(expected, blob.GetBytes());
        }

        [TestMethod]
        public void TestAddStringForUtf8()
        {
            var blob = new StringBlob(Encoding.UTF8);
            var offset = blob.AddString("Poop 💩!");
            Assert.AreEqual(0, offset);
            Assert.AreEqual(11, blob.Length);

            var expected = new byte[] { 0x50, 0x6F, 0x6F, 0x70, 0x20, 0xF0, 0x9F, 0x92, 0xA9, 0x21, 0x00 };
            CollectionAssert.AreEqual(expected, blob.GetBytes());
        }

        [TestMethod]
        public void TestAddStringForUtf16()
        {
            var blob = new StringBlob(Encoding.Unicode);
            var offset = blob.AddString("Poop 💩!");
            Assert.AreEqual(0, offset);
            Assert.AreEqual(18, blob.Length);

            var expected = new byte[] { 0x50, 0x00, 0x6F, 0x00, 0x6F, 0x00, 0x70, 0x00, 0x20, 0x00, 0x3D, 0xD8, 0xA9, 0xDC, 0x21, 0x00, 0x00, 0x00 };
            CollectionAssert.AreEqual(expected, blob.GetBytes());
        }
    }
}

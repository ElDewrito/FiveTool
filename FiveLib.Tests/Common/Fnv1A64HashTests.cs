using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FiveLib.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FiveLib.Tests.Common
{
    [TestClass]
    public class Fnv1A64HashTests
    {
        // Expected hash values obtained from http://www.isthe.com/chongo/src/fnv/test_fnv.c

        [TestMethod]
        public void TestFnv1A64HashComputeHash()
        {
            var helloBytes = Encoding.ASCII.GetBytes("foobar");
            var expectedHash = new byte[] { 0x85, 0x94, 0x41, 0x71, 0xF7, 0x39, 0x67, 0xE8 };
            var actualHash = new Fnv1A64Hash().ComputeHash(helloBytes);
            CollectionAssert.AreEqual(expectedHash, actualHash);
        }

        [TestMethod]
        public void TestFnv1A64HashComputeValue()
        {
            var stringBytes = Encoding.ASCII.GetBytes("foobar");
            var expectedHash = 0x85944171F73967E8UL;
            var actualHash = Fnv1A64Hash.ComputeValue(stringBytes);
            Assert.AreEqual(expectedHash, actualHash);
        }
    }
}

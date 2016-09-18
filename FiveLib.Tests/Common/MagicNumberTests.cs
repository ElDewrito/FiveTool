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
    public class MagicNumberTests
    {
        [TestMethod]
        public void TestConstructingFromInteger()
        {
            var magic = new MagicNumber(42);
            Assert.AreEqual(42, magic.Value);
        }

        [TestMethod]
        public void TestConstructingFromString()
        {
            var magic1 = new MagicNumber("abcd");
            Assert.AreEqual(0x61626364, magic1.Value, "4-char string");

            var magic2 = new MagicNumber("abc");
            Assert.AreEqual(0x616263, magic2.Value, "Short string");

            var magic3 = new MagicNumber("abcde");
            Assert.AreEqual(0x62636465, magic3.Value, "Long string");

            var magic4 = new MagicNumber("");
            Assert.AreEqual(0, magic4.Value, "Empty string");
        }

        [TestMethod]
        public void TestToString()
        {
            var magic1 = new MagicNumber("abcd");
            Assert.AreEqual("abcd", magic1.ToString(), "4-char string");

            var magic2 = new MagicNumber("abc");
            Assert.AreEqual("abc", magic2.ToString(), "Short string");

            var magic3 = new MagicNumber("abcde");
            Assert.AreEqual("bcde", magic3.ToString(), "Long string");

            var magic4 = new MagicNumber("");
            Assert.AreEqual("", magic4.ToString(), "Empty string");
        }

        [TestMethod]
        public void TestEquals()
        {
            Assert.AreEqual(new MagicNumber(42), new MagicNumber(42));
            Assert.AreNotEqual(new MagicNumber(42), new MagicNumber(43));
        }

        [TestMethod]
        public void TestCompareTo()
        {
            var magic42 = new MagicNumber(42);
            var magic43 = new MagicNumber(43);
            Assert.IsTrue(magic42.CompareTo(magic43) < 0);
            Assert.IsTrue(magic43.CompareTo(magic42) > 0);
            Assert.IsTrue(magic42.CompareTo(new MagicNumber(42)) == 0);
        }
    }
}

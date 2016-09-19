using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FiveLib.Ausar.Tags;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FiveLib.Tests.Ausar.Tags
{
    [TestClass]
    public class StringIdTests
    {
        [TestMethod]
        public void TestConstructStringIdFromUInt()
        {
            var stringId = new StringId(1337);
            Assert.AreEqual(1337U, stringId.Value);
        }

        [TestMethod]
        public void TestConstructStringIdFromString()
        {
            // We can assume that the murmurhash library works, so this doesn't need to be very extensive
            var stringId = new StringId("__default__");
            Assert.AreEqual(stringId.Value, 0x9B555AD2);
        }
    }
}

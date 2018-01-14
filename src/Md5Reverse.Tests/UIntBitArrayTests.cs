using Md5Reverse.Lib.Utils;
using NUnit.Framework;
using NUnit.Framework.Internal;
using System;

namespace Md5Reverse.Tests
{
    [TestFixture]
    public class UIntBitArrayTests
    {
        [Test]
        public void SingleSetTest()
        {
            var arr = new UIntBitArray();

            arr.Set(UInt32.MaxValue, true);
            arr.Set(UInt32.MaxValue - 1, true);
            arr.Set((uint)Int32.MaxValue + 1, true);
            arr.Set((uint)Int32.MaxValue - 1, true);
            arr.Set((uint)Int32.MaxValue, true);
            arr.Set(10, true);

            Assert.IsTrue(arr.Get(UInt32.MaxValue));


            //for (uint i = 0; i < uint.MaxValue; i += 4)
            //{
            //    Assert.IsFalse(arr.Get(i), $"Index: {i}");
            //}
        }
    }
}
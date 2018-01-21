using Md5Reverse.Lib;
using NUnit.Framework;

namespace Md5Reverse.Tests
{
    [TestFixture]
    public class OptimizedHashProviderTests
    {
        [Test]
        public void CorrectnessUint32_Single()
        {
            var alg = new OptimizedHashProvider();
            var stand = new StandardHashProvider();

            uint item = 256 + 512 + 32;// UInt32.MaxValue / 2;

            var expected = stand.ComputeUIntHash(item);
            var actual = alg.ComputeUIntHash(item);
            Assert.AreEqual(expected, actual);

            expected = stand.ComputeUIntHash(item / 2);
            actual = alg.ComputeUIntHash(item / 2);
            Assert.AreEqual(expected, actual);

        }


        [Test]
        public void CorrectnessUint32()
        {
            var alg = new OptimizedHashProvider();
            var stand = new StandardHashProvider();

            for (uint i = 0; i < 10000; i++)
            {
                var expected = stand.ComputeUIntHash(i);
                var actual = alg.ComputeUIntHash(i);
                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        public void CorrectnessLong()
        {
            var alg = new OptimizedHashProvider();
            var stand = new StandardHashProvider();

            long lmin = 0x0110000100000000;

            for (long i = 0; i < 10000; i++)
            {
                var expected = stand.ComputeByteHash(i + lmin);
                var actual = alg.ComputeByteHash(i + lmin);
                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        public void CorrectnessBytes()
        {
            var alg = new OptimizedHashProvider();
            var stand = new StandardHashProvider();

            for (uint i = 0; i < 10000; i++)
            {
                var expected = stand.ComputeByteHash(i);
                var actual = alg.ComputeByteHash(i);

                for (var j = 0; j < 4; j++)
                {
                    Assert.AreEqual(expected[j], actual[j]);
                }
            }
        }
    }
}
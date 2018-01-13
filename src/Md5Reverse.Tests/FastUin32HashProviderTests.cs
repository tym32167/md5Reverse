using Md5Reverse.Lib;
using NUnit.Framework;

namespace Md5Reverse.Tests
{
    [TestFixture]
    public class FastUin32HashProviderTests
    {
        [Test]
        public void CorrectnessUint32()
        {
            var alg = new FastUin32HashProvider();
            var stand = new StandardHashProvider();

            for (uint i = 0; i < 10000; i++)
            {
                var expected = stand.ComputeUIntHash(i);
                var actual = alg.ComputeUIntHash(i);
                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        public void CorrectnessBytes()
        {
            var alg = new FastUin32HashProvider();
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
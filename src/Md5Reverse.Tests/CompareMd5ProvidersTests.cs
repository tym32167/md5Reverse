using Md5Reverse.Lib;
using NUnit.Framework;
using NUnit.Framework.Internal;
using System.Diagnostics;

namespace Md5Reverse.Tests
{
    [TestFixture]
    public class CompareMd5ProvidersTests
    {
        [Test]
        [Ignore("")]
        public void CompareUintHash()
        {
            var alg = new FastUin32HashProvider();
            var stand = new StandardHashProvider();
            uint iterations = 1000000;

            var sw = new Stopwatch();
            sw.Start();
            for (uint i = 0; i <= iterations; i++)
            {
                alg.ComputeUIntHash(i);
            }
            sw.Stop();
            var algElapsed = sw.Elapsed;


            sw.Reset();
            sw.Start();
            for (uint i = 0; i <= iterations; i++)
            {
                stand.ComputeUIntHash(i);
            }
            sw.Stop();
            var standElapsed = sw.Elapsed;

            System.Console.WriteLine($"fast: {algElapsed}, stand: {standElapsed}");
            Assert.True(standElapsed > algElapsed, $"{nameof(algElapsed)} should be faster than {nameof(standElapsed)}");
        }


        [Test]
        public void CompareUintHash_fast_performance()
        {
            var alg = new FastUin32HashProvider();
            uint iterations = 6000;
            var sw = new Stopwatch();

            sw.Start();
            for (uint i = 0; i <= iterations; i++)
            {
                alg.ComputeUIntHash(i);
            }
            sw.Stop();
            var algElapsed = sw.Elapsed;

            System.Console.WriteLine($"fast: {algElapsed}");
        }
    }
}
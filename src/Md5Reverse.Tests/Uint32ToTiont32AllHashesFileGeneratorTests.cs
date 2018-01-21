using Md5Reverse.Console;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace Md5Reverse.Tests
{
    [TestFixture]
    public class Uint32ToTiont32AllHashesFileGeneratorTests
    {
        [Test]
        public void SortByHashTest()
        {
            var source = new byte[8 * 3];
            var temp = new byte[8 * 3];

            var indexes = new uint[256 * 256 * 256];

            for (byte i = 8 * 3 - 1; i > -0; i--)
                source[8 * 3 - 1 - i] = i;

            Uint32ToTiont32AllHashesFileGenerator.CreateMathes(source, 8 * 3, indexes);
            Uint32ToTiont32AllHashesFileGenerator.CreateIndexes(indexes);
            Uint32ToTiont32AllHashesFileGenerator.SortByHash(source, temp, 8 * 3, indexes);

            var expected = new byte[] { 7, 6, 5, 4, 3, 2, 1, 0, 15, 14, 13, 12, 11, 10, 9, 8, 23, 22, 21, 20, 19, 18, 17, 16 };

            CollectionAssert.AreEqual(expected, temp);
        }

        [Test]
        public void IndexesByHashTest()
        {
            var source = new byte[8 * 3];

            var indexes = new uint[256 * 256 * 256];

            for (byte i = 8 * 3 - 1; i > -0; i--)
                source[8 * 3 - 1 - i] = i;

            Uint32ToTiont32AllHashesFileGenerator.CreateMathes(source, 8 * 3, indexes);
            Uint32ToTiont32AllHashesFileGenerator.CreateIndexes(indexes);

            var count765 = indexes[7 * 256 * 256 + 6 * 256 + 5];
            var count151413 = indexes[15 * 256 * 256 + 14 * 256 + 13];
            var count232221 = indexes[23 * 256 * 256 + 22 * 256 + 21];


            Assert.AreEqual(0, count765);
            Assert.AreEqual(8, count151413);
            Assert.AreEqual(16, count232221);
        }

        [Test]
        public void IndexesByRepeatedHashTest()
        {
            var source = new byte[8 * 4];

            var indexes = new uint[256 * 256 * 256];

            for (byte i = 8 * 4 - 1; i > -0; i--)
                source[i] = (byte)(i % 4);

            Uint32ToTiont32AllHashesFileGenerator.CreateMathes(source, 8 * 4, indexes);
            Uint32ToTiont32AllHashesFileGenerator.CreateIndexes(indexes);
            var count012 = indexes[0 * 256 * 256 + 1 * 256 + 2];
            Assert.AreEqual(0, count012);
        }
    }
}
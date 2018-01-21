using Md5Reverse.Console;
using Md5Reverse.Lib;
using Md5Reverse.Lib.Core;
using Md5Reverse.Lib.Utils;
using NUnit.Framework;
using NUnit.Framework.Internal;
using System;
using System.Diagnostics;
using System.IO;

namespace Md5Reverse.Tests
{
    [TestFixture]
    public class Uint32ToTiont32AllHashesFileGeneratorTests
    {
        [Test]
        public void SortByHashTest()
        {
            var md5 = new OptimizedHashProvider();
            var log = new FakeLog();
            var generator = new Uint32ToTiont32AllHashesFileGenerator(md5, log);


            var source = new byte[8 * 3];
            var temp = new byte[8 * 3];

            var indexes = new uint[256 * 256 * 256];

            for (byte i = 8 * 3 - 1; i > -0; i--)
                source[8 * 3 - 1 - i] = i;

            generator.CreateMathes(source, 8 * 3, indexes);
            generator.CreateIndexes(indexes);
            generator.SortByHash(source, temp, 8 * 3, indexes);

            var expected = new byte[] { 7, 6, 5, 4, 3, 2, 1, 0, 15, 14, 13, 12, 11, 10, 9, 8, 23, 22, 21, 20, 19, 18, 17, 16 };

            CollectionAssert.AreEqual(expected, temp);
        }

        [Test]
        public void SwapTest()
        {
            var md5 = new OptimizedHashProvider();
            var log = new FakeLog();
            var generator = new Uint32ToTiont32AllHashesFileGenerator(md5, log);


            var source = new byte[8 * 2];

            for (byte i = 8 * 2 - 1; i > -0; i--)
                source[i] = i;

            generator.Swap(source, 0, 8);

            var expected = new byte[] { 8, 9, 10, 11, 12, 13, 14, 15, 0, 1, 2, 3, 4, 5, 6, 7 };

            CollectionAssert.AreEqual(expected, source);
        }

        [Test]
        public void IndexesByHashTest()
        {
            var md5 = new OptimizedHashProvider();
            var log = new FakeLog();
            var generator = new Uint32ToTiont32AllHashesFileGenerator(md5, log);


            var source = new byte[8 * 3];

            var indexes = new uint[256 * 256 * 256];

            for (byte i = 8 * 3 - 1; i > -0; i--)
                source[8 * 3 - 1 - i] = i;

            generator.CreateMathes(source, 8 * 3, indexes);
            generator.CreateIndexes(indexes);

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
            var md5 = new OptimizedHashProvider();
            var log = new FakeLog();
            var generator = new Uint32ToTiont32AllHashesFileGenerator(md5, log);

            var source = new byte[8 * 4];

            var indexes = new uint[256 * 256 * 256];

            for (byte i = 8 * 4 - 1; i > -0; i--)
                source[i] = (byte)(i % 4);

            generator.CreateMathes(source, 8 * 4, indexes);
            generator.CreateIndexes(indexes);
            var count012 = indexes[0 * 256 * 256 + 1 * 256 + 2];
            Assert.AreEqual(0, count012);
        }

        [Test]
        public void SortingTest()
        {
            var md5 = new OptimizedHashProvider();
            var log = new FakeLog();
            var indexes = new uint[256 * 256 * 256];
            var generator = new Uint32ToTiont32AllHashesFileGenerator(md5, log);

            var fileBytes = File.ReadAllBytes(@"G:\temp\FinalVariant3\step1\127.bin");
            var temp = new byte[fileBytes.Length];

            generator.SortFileData(indexes, fileBytes, temp, fileBytes.Length);

            for (var i = 8; i < temp.Length; i += 8)
            {
                var prev = temp[i - 8] * 256 * 256 + temp[i - 8 + 1] * 256 + temp[i - 8 + 2];
                var next = temp[i] * 256 * 256 + temp[i + 1] * 256 + temp[i + 2];

                Assert.GreaterOrEqual(next, prev, $"error on position {i}");
            }

        }


        [Test]
        public void IndexesBigTest()
        {
            var md5 = new OptimizedHashProvider();
            var log = new Log();
            var indexes = IdByHashSearcher.GetCache(@"G:\temp\FinalVariant3\step5\index.bin");

            var idsFile = @"G:\temp\FinalVariant3\step5\ids.bin";

            uint position = 0;

            log.Info("Starting...");


            using (var idReader = idsFile.CreateReader().WithSpy(log).Buffered().ToBinaryReader())
            {
                while (idReader.BaseStream.Position < idReader.BaseStream.Length)
                {
                    var id = idReader.ReadUInt32();
                    var hash = md5.ComputeByteHash(id);

                    var ind = hash[0] * 256 * 256 + hash[1] * 256 + hash[2];

                    var idxPos = indexes[ind];


                    Assert.GreaterOrEqual(position, idxPos);

                    if (idxPos < indexes.Length)
                    {
                        var idxPosNext = indexes[ind + 1];
                        Assert.Less(position, idxPosNext);
                    }

                    position++;



                    if ((position + 1) / 10000000 > position / 10000000)
                    {
                        var percent = position * 100.0 / uint.MaxValue;
                        log.Info($"Percent done: {percent}, current: {position}");
                    }

                }

            }



        }

        public class FakeLog : ILog
        {
            public void Debug(object message)
            {
                System.Diagnostics.Debug.WriteLine($"{DateTime.UtcNow:T} Debug: {message}");
            }

            public void Info(object message)
            {
                System.Diagnostics.Debug.WriteLine($"{DateTime.UtcNow:T} Info: {message}");
            }

            public void Error(object message)
            {
                System.Diagnostics.Debug.WriteLine($"{DateTime.UtcNow:T} Error: {message}");
            }

            public void Fatal(object message)
            {
                System.Diagnostics.Debug.WriteLine($"{DateTime.UtcNow:T} Fatal: {message}");
            }

            public IDisposable Timing(string text)
            {
                return new TimingHelper(text, this);
            }

            private class TimingHelper : IDisposable
            {
                private readonly string _text;
                private readonly ILog _log;
                private readonly Stopwatch _watch = new Stopwatch();

                public TimingHelper(string text, ILog log)
                {
                    _text = text;
                    _log = log;

                    _log.Info($"{text} started.");

                    _watch.Start();
                }

                public void Dispose()
                {
                    _watch.Stop();

                    _log.Info($"{_text} finished. Timing: {_watch.Elapsed}.");
                }
            }
        }
    }
}
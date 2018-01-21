using Md5Reverse.Lib;
using Md5Reverse.Lib.Core;
using Md5Reverse.Lib.Utils;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Md5Reverse.Console
{
    public class Program
    {
        static void Main(string[] args)
        {
            //ComputeChans();
            //CreateDictFromFile();

            //TEST();

            //NumDuplicates();

            //GenerateAllHashes();

            //ChainsExperiment.ComputeChanis();

            //SplitSelf();
            //Split();


            var log = new Log();
            var md5 = new StandardHashProvider();

            var searcher = new IdByHashSearcher(@"G:\temp\FinalVariant3\step5\ids.bin", @"G:\temp\FinalVariant3\step5\index.bin", md5, log);

            //var ids = File.ReadAllLines(@"G:\temp\testdata.txt");
            //var result = searcher.Search(ids);
            //File.WriteAllLines(@"G:\temp\idsAndHashes.txt", result.Select(x => $"{x.Key}|{x.Value}"));



            searcher.Search(new[] { "00020000ca27ac810c1a5ff984dc8e69" });
            searcher.Search(new[] { "73f07ef50e3d97aa376c56795481c341" });



            //var log = new Log();
            //var md5 = new OptimizedHashProvider();
            //var generator = new Uint32ToTiont32AllHashesFileGenerator(md5, log);
            //generator.GenerateFile(@"G:\temp\FinalVariant3");


            //TestIndexes();
        }

        private static void TestIndexes()
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

                    if (position < idxPos)
                    {
                        log.Info($"-------- OMG {position} / {idxPos}");
                        return;
                    }

                    if (ind < indexes.Length - 1)
                    {
                        var idxPosNext = indexes[ind + 1];

                        if (idxPosNext <= position)
                        {
                            log.Info($"-------- OMG idxPosNext {position} / {idxPosNext}");
                            return;
                        }
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


        static void GenerateAllHashes()
        {
            var md5 = new FastUin32HashProvider();
            var log = new Log();
            var folder = @"G:\temp\FinalVariant\";

            var generator = new Uint32ToTiont32AllHashesFileGenerator(md5, log);
            generator.GenerateFile(folder);
        }


        static void SplitSelf()
        {
            var log = new Log();
            var dict = new Dictionary<uint, uint>();
            var provider = new FastUin32HashProvider();

            using (log.Timing(nameof(SplitSelf)))
            {
                for (var i = 1; i <= 82; i++)
                {
                    dict.Clear();

                    var filename = $"{i}.bin";
                    var fullIn = Path.Combine("G:\\temp\\ChainGen\\Splitted\\", filename);
                    var fullOut = Path.Combine("G:\\temp\\ChainGen\\Splitted_2\\", filename);

                    if (File.Exists(fullOut)) continue;

                    log.Info($"Starting {fullIn}");

                    using (var reader = File.OpenRead(fullIn).WithSpy(log).Buffered(1024 * 1024 * 50)
                        .ToBinaryReader())
                    {
                        while (reader.BaseStream.Position < reader.BaseStream.Length)
                        {
                            var start = reader.ReadUInt32();
                            var end = reader.ReadUInt32();

                            dict.Add(start, end);
                        }
                    }

                    var total = dict.Count;

                    var kvalues = dict.ToArray();

                    var ind = 0;
                    var found = 0;


                    foreach (var kv in kvalues)
                    {
                        ind++;

                        var end = kv.Value;

                        for (var j = 0; j < 100; j++)
                        {
                            if (dict.ContainsKey(end))
                            {
                                var newEnd = dict[end];
                                dict.Remove(end);
                                dict.Remove(kv.Key);
                                dict.Add(kv.Key, newEnd);
                                found++;
                                break;
                            }

                            end = provider.ComputeUIntHash(end);
                        }

                        if ((ind + 1) / 20000 > ind / 20000)
                        {
                            var percent = 100.0 * ind / total;
                            log.Info($"Percent done: {percent}, ind:{ind}, found:{found}");
                        }
                    }

                    var newtotal = dict.Count;


                    log.Info($"Writing {fullOut}");

                    using (var writer = fullOut.CreateWriter().WithSpy(log).Buffered(1024 * 1024 * 50).ToBinaryWriter())
                    {
                        foreach (var kv in dict)
                        {
                            writer.Write(kv.Key);
                            writer.Write(kv.Value);
                        }
                    }

                    log.Info($"Total {total}, mathed {total - newtotal}, percent {100.0 * (total - newtotal) / total}");
                }
            }
        }


        static void Split()
        {
            var log = new Log();
            var buffer = new byte[200 * 1024 * 1024];
            using (log.Timing(nameof(Split)))
            {
                using (var reader = File.OpenRead(@"G:\temp\ChainGen\chain_pure5.bin").WithSpy(log))
                {
                    var i = 0;

                    int len;
                    do
                    {
                        len = reader.Read(buffer, 0, buffer.Length);
                        if (len > 0)
                        {
                            i++;

                            using (var fw = File.OpenWrite($"G:\\temp\\ChainGen\\Splitted\\{i}.bin").WithSpy(log))
                            {
                                fw.Write(buffer, 0, len);
                            }
                        }
                    } while (len > 0);
                }
            }
        }
    }
}

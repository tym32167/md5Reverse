using Md5Reverse.Lib.Core;
using Md5Reverse.Lib.Utils;
using System;
using System.Collections.Generic;
using System.IO;

namespace Md5Reverse.Console
{
    public class Uint32ToTiont32AllHashesFileGenerator
    {
        private readonly IMd5Provider _md5Provider;
        private readonly ILog _log;

        public Uint32ToTiont32AllHashesFileGenerator(IMd5Provider md5Provider, ILog log)
        {
            _md5Provider = md5Provider;
            _log = log;
        }

        public void GenerateFile(string folder)
        {
            var step1Folder = Path.Combine(folder, "step1");
            Step1_Generate256UnsortedFiles(step1Folder);


            var step2Folder = Path.Combine(folder, "step2");
            var step3Folder = Path.Combine(folder, "step3");

            Step2_Generate256SortedFiles(step1Folder, step2Folder, step3Folder);

            Step3_MergeSortedFiles(step2Folder, step3Folder);

            Step4_GenerateIndexFile(step3Folder);
        }

        private void Step4_GenerateIndexFile(string step3Folder)
        {
            var hashesFile = Path.Combine(step3Folder, "ids.bin");
            var IndexFile = Path.Combine(step3Folder, "index.bin");
        }

        private void Step3_MergeSortedFiles(string step2Folder, string step3Folder)
        {
            if (!Directory.Exists(step3Folder))
                Directory.CreateDirectory(step3Folder);

            // Timing: 0:13:42
            using (_log.Timing(nameof(Step3_MergeSortedFiles)))
            {
                var outFile = Path.Combine(step3Folder, "ids.bin");

                if (File.Exists(outFile))
                {
                    _log.Info($"Step 3: File {outFile} already exisis.");
                    return;
                }

                var buffer = new byte[1024 * 1024 * 140];

                using (var writer = outFile.CreateWriter().WithSpy(_log).Buffered())
                {
                    for (var i = 0; i < 256; i++)
                    {
                        var fileName = $"{i}.bin";
                        var inputFile = Path.Combine(step2Folder, fileName);

                        _log.Info($"Processing {fileName}");

                        using (var reader = inputFile.CreateReader().WithSpy(_log))
                        {
                            int len;
                            do
                            {
                                len = reader.Read(buffer, 0, buffer.Length);
                                for (var j = 0; j < len; j += 8)
                                {
                                    writer.Write(buffer, j + 4, 4);
                                }
                            } while (len > 0);
                        }
                    }
                }
            }
        }


        private void Step2_Generate256SortedFiles(string step1Folder, string destFolder, string indexFolder)
        {
            if (!Directory.Exists(destFolder))
                Directory.CreateDirectory(destFolder);

            if (!Directory.Exists(indexFolder))
                Directory.CreateDirectory(indexFolder);

            // Timing: 0:32:36
            using (_log.Timing(nameof(Step2_Generate256SortedFiles)))
            {
                var dataList = new List<KeyValuePair<uint, uint>>();
                var comparer = new KeyValueComparer();
                var buffer = new byte[1024 * 1024 * 140];

                var indexFileName = "index.bin";

                using (var indexWriter = Path.Combine(indexFolder, indexFileName).CreateWriter().WithSpy(_log)
                    .Buffered(10 * 1024 * 1024).ToBinaryWriter())
                {
                    var prevHash = 0u;
                    var line = 0u;

                    for (var i = 0; i < 256; i++)
                    {
                        var fileName = $"{i}.bin";

                        var inputFile = Path.Combine(step1Folder, fileName);
                        var outputFile = Path.Combine(destFolder, fileName);

                        if (File.Exists(outputFile)) continue;

                        dataList.Clear();

                        _log.Info($"Processing {fileName}");

                        using (var reader = inputFile.CreateReader().WithSpy(_log))
                        {
                            int len;
                            do
                            {
                                len = reader.Read(buffer, 0, buffer.Length);
                                for (var j = 0; j < len; j += 8)
                                {
                                    var ind = BitConverter.ToUInt32(buffer, j);
                                    var hash = BitConverter.ToUInt32(buffer, j + 4);
                                    dataList.Add(new KeyValuePair<uint, uint>(hash, ind));
                                }
                            } while (len > 0);
                        }

                        using (_log.Timing("Sorting"))
                        {
                            dataList.Sort(comparer);
                        }

                        using (var writer = outputFile.CreateWriter().WithSpy(_log).Buffered(20 * 1024 * 1024))
                        {
                            foreach (var kv in dataList)
                            {
                                writer.Write(BitConverter.GetBytes(kv.Key), 0, 4);
                                writer.Write(BitConverter.GetBytes(kv.Value), 0, 4);

                                if (prevHash >> 8 != kv.Key >> 8)
                                {
                                    prevHash = kv.Key;
                                    indexWriter.Write(line);
                                }

                                line++;
                            }
                        }
                    }
                }
            }
        }

        private class KeyValueComparer : IComparer<KeyValuePair<uint, uint>>
        {
            public int Compare(KeyValuePair<uint, uint> x, KeyValuePair<uint, uint> y)
            {
                return x.Key.CompareTo(y.Key);
            }
        }

        private void Step1_Generate256UnsortedFiles(string folder)
        {
            if (Directory.Exists(folder))
            {
                _log.Info($"Unable to perform step1 - folder {folder} already exists");
                return;
            }

            Directory.CreateDirectory(folder);

            // Timing: 1:17:37
            using (_log.Timing(nameof(Step1_Generate256UnsortedFiles)))
            {
                BinaryWriter[] writers = new BinaryWriter[256];

                try
                {
                    for (var i = 0; i < writers.Length; i++)
                    {
                        var fname = $"{i}.bin";
                        var path = Path.Combine(folder, fname);
                        writers[i] = path.CreateWriter().WithSpy(_log).Buffered(1 * 1024 * 1024).ToBinaryWriter();
                    }

                    for (uint i = 0; ; i++)
                    {
                        var hash = _md5Provider.ComputeByteHash(i);
                        var uintvalue = BitConverter.ToUInt32(hash, 0);

                        var writer = writers[hash[0]];
                        writer.Write(i);
                        writer.Write(uintvalue);

                        if ((i + 1) / 10000000 > i / 10000000)
                        {
                            var percent = i * 100.0 / uint.MaxValue;
                            _log.Info($"Percent done: {percent}, current: {i}");
                        }

                        if (i == uint.MaxValue) break;
                    }
                }
                catch (Exception ex)
                {
                    _log.Error(ex);
                }
                finally
                {
                    foreach (var writer in writers)
                        writer?.Dispose();
                }
            }
        }
    }
}

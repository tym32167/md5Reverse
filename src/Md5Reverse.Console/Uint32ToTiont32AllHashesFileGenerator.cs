using Md5Reverse.Lib.Core;
using Md5Reverse.Lib.Utils;
using System;
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
        }

        private void Step1_Generate256UnsortedFiles(string folder)
        {
            if (Directory.Exists(folder))
            {
                _log.Info($"Unable to perform step1 - folder {folder} already exists");
                return;
            }

            Directory.CreateDirectory(folder);

            using (_log.Timing(nameof(Step1_Generate256UnsortedFiles)))
            {
                BinaryWriter[] writers = new BinaryWriter[256];

                try
                {
                    for (var i = 0; i < writers.Length; i++)
                    {
                        var fname = $"step1_{i}.bin";
                        var path = Path.Combine(folder, fname);
                        writers[i] = FileReaderWriterFactory.CreateBinaryWriter(path, _log, 1 * 1024 * 1024);
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

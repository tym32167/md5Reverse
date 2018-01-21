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

            //var step2Folder = Path.Combine(folder, "step2");
            //var step3Folder = Path.Combine(folder, "step3");

            //Step2_Generate256SortedFiles(step1Folder, step2Folder);
            //Step3_MergeSortedFiles(step2Folder, step3Folder);
            //Step4_GenerateIndexFile(step2Folder, step3Folder);


            var step5Folder = Path.Combine(folder, "step5");
            Step5_GenerateIndexAndIdsFile(step1Folder, step5Folder);
        }

        private void Step5_GenerateIndexAndIdsFile(string step1Folder, string step5Folder)
        {
            if (!Directory.Exists(step5Folder))
                Directory.CreateDirectory(step5Folder);

            // Timing: 0:12:37
            using (_log.Timing(nameof(Step5_GenerateIndexAndIdsFile)))
            {
                var idsFile = Path.Combine(step5Folder, "ids.bin");

                using (var idsWriter = idsFile.CreateWriter().WithSpy(_log).Buffered())
                {
                    var indexFileName = Path.Combine(step5Folder, "index.bin");

                    var fileIndexes = new uint[256 * 256 * 256];
                    var filebuffer = new byte[256 * 256 * 256 * 8 + 1024 * 1024 * 2];
                    var tempBuffer = new byte[256 * 256 * 256 * 8 + 1024 * 1024 * 2];

                    var indexes = new uint[256 * 256 * 256];

                    for (var i = 0; i < 256; i++)
                    {
                        var fileName = $"{i}.bin";
                        var inputFile = Path.Combine(step1Folder, fileName);

                        _log.Info($"Processing {fileName}");

                        int inputLen;

                        using (var sr = inputFile.CreateReader().WithSpy(_log))
                        {
                            inputLen = sr.Read(filebuffer, 0, filebuffer.Length);
                        }

                        CreateMathes(filebuffer, inputLen, indexes);

                        Clear(fileIndexes);
                        CreateMathes(filebuffer, inputLen, fileIndexes);
                        CreateIndexes(fileIndexes);
                        SortByHash(filebuffer, tempBuffer, inputLen, fileIndexes);

                        for (var j = 0; j < inputLen; j += 8)
                        {
                            idsWriter.Write(tempBuffer, j + 4, 4);
                        }
                    }

                    CreateIndexes(indexes, 1);

                    using (var indexWriter = indexFileName.CreateWriter().WithSpy(_log).Buffered(10 * 1024 * 1024)
                        .ToBinaryWriter())
                    {
                        for (var i = 0; i < 256; i++)
                        {
                            for (var ii = 0; ii < 256; ii++)
                            {
                                for (var jj = 0; jj < 256; jj++)
                                {
                                    var ind = 256 * 256 * i + 256 * ii + jj;
                                    indexWriter.Write(indexes[ind]);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void Step4_GenerateIndexFile(string step2Folder, string indexFolder)
        {
            if (!Directory.Exists(indexFolder))
                Directory.CreateDirectory(indexFolder);

            using (_log.Timing(nameof(Step4_GenerateIndexFile)))
            {
                var indexFileName = Path.Combine(indexFolder, "index.bin");

                if (File.Exists(indexFileName))
                {
                    _log.Info($"Step 4: File {indexFileName} already exisis.");
                    return;
                }

                using (var writer = indexFileName.CreateWriter().WithSpy(_log).Buffered(10 * 1024 * 1024).ToBinaryWriter())
                {
                    var filebuffer = new byte[256 * 256 * 256 * 8 + 1024 * 1024 * 2];
                    var indexes = new uint[256 * 256 * 256];

                    for (var i = 0; i < 256; i++)
                    {
                        var fileName = $"{i}.bin";
                        var inputFile = Path.Combine(step2Folder, fileName);

                        _log.Info($"Processing {fileName}");

                        int inputLen;

                        using (var sr = inputFile.CreateReader().WithSpy(_log))
                        {
                            inputLen = sr.Read(filebuffer, 0, filebuffer.Length);
                        }

                        CreateMathes(filebuffer, inputLen, indexes);
                    }

                    CreateIndexes(indexes);

                    for (var i = 0; i < 256; i++)
                    {
                        for (var ii = 0; ii < 256; ii++)
                        {
                            for (var jj = 0; jj < 256; jj++)
                            {
                                var ind = 256 * 256 * i + 256 * ii + jj;
                                writer.Write(indexes[ind]);
                            }
                        }
                    }
                }
            }

        }

        private void Step3_MergeSortedFiles(string step2Folder, string step3Folder)
        {
            if (!Directory.Exists(step3Folder))
                Directory.CreateDirectory(step3Folder);

            // Timing: 0:12:42
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

        private void Step2_Generate256SortedFiles(string step1Folder, string destFolder)
        {
            if (!Directory.Exists(destFolder))
                Directory.CreateDirectory(destFolder);

            // Timing: 0:12:37
            using (_log.Timing(nameof(Step2_Generate256SortedFiles)))
            {
                var indexes = new uint[256 * 256 * 256];
                var filebuffer = new byte[256 * 256 * 256 * 8 + 1024 * 1024 * 2];
                var tempBuffer = new byte[256 * 256 * 256 * 8 + 1024 * 1024 * 2];

                for (var i = 0; i < 256; i++)
                {

                    var fileName = $"{i}.bin";

                    var inputFile = Path.Combine(step1Folder, fileName);
                    var outputFile = Path.Combine(destFolder, fileName);


                    if (File.Exists(outputFile)) continue;

                    _log.Info($"Processing {fileName}");

                    int inputLen;

                    using (var sr = inputFile.CreateReader().WithSpy(_log))
                    {
                        inputLen = sr.Read(filebuffer, 0, filebuffer.Length);
                    }

                    SortFileData(indexes, filebuffer, tempBuffer, inputLen);

                    using (var sw = outputFile.CreateWriter().WithSpy(_log))
                    {
                        sw.Write(tempBuffer, 0, inputLen);
                    }
                }
            }
        }

        internal void SortFileData(uint[] indexes, byte[] filebuffer, byte[] tempBuffer, int inputLen)
        {
            Clear(indexes);
            CreateMathes(filebuffer, inputLen, indexes);
            CreateIndexes(indexes);
            SortByHash(filebuffer, tempBuffer, inputLen, indexes);
        }

        private void Clear(uint[] source)
        {
            for (var i = 0; i < source.Length; i++)
                source[i] = 0;
        }



        internal void CreateIndexes(uint[] indexes, uint wordLen = 8)
        {
            var curPos = 0u;
            for (var j = 0; j < indexes.Length; j++)
            {
                var count = indexes[j];
                indexes[j] = curPos * wordLen;
                curPos += count;
            }
        }

        internal void CreateMathes(byte[] inputBytes, int inputLen, uint[] indexes)
        {
            var rowCount = inputLen / 8;

            for (var j = 0; j < rowCount; j++)
            {
                var inpInd = j * 8;

                var i1 = inputBytes[inpInd];
                var i2 = inputBytes[inpInd + 1];
                var i3 = inputBytes[inpInd + 2];

                indexes[i1 * 256 * 256 + i2 * 256 + i3]++;
            }
        }

        internal void SortByHash(byte[] inputBytes, byte[] tempBuffer, int inputLen, uint[] indexes)
        {
            var rowCount = inputLen / 8;
            for (uint j = 0; j < rowCount; j++)
            {
                var inpInd = j * 8;

                var i1 = inputBytes[inpInd];
                var i2 = inputBytes[inpInd + 1];
                var i3 = inputBytes[inpInd + 2];

                var nextInd = indexes[i1 * 256 * 256 + i2 * 256 + i3];
                Copy(inputBytes, tempBuffer, inpInd, nextInd);
                indexes[i1 * 256 * 256 + i2 * 256 + i3] += 8;
            }
        }

        internal void Copy(byte[] source, byte[] dest, uint i, uint j)
        {
            for (var k = 0; k < 8; k++)
            {
                var index0 = i + k;
                var index1 = j + k;

                if (index0 >= source.Length) break;
                if (index1 >= source.Length) break;

                dest[index1] = source[index0];
            }
        }

        internal void Swap(byte[] source, uint i, uint j)
        {
            for (var k = 0; k < 8; k++)
            {
                var index0 = i + k;
                var index1 = j + k;

                if (index0 >= source.Length) break;
                if (index1 >= source.Length) break;

                var temp = source[index0];
                source[index0] = source[index1];
                source[index1] = temp;
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

            // Timing: 0:20:37
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
                        var writer = writers[hash[0]];

                        writer.Write(hash, 0, 4);
                        writer.Write(i);


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

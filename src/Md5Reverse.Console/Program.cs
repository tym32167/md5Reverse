using Md5Reverse.Lib;
using Md5Reverse.Lib.Core;
using System;

namespace Md5Reverse.Console
{
    public class Program
    {
        static void Main(string[] args)
        {
            //SearchMd5Test();
            GenerateFiles();
        }

        private static void SearchMd5Test()
        {
            var log = new Log();
            var md5 = new StandardHashProvider();

            var searcher = new IdByHashSearcher(@"G:\temp\FinalVariant\ids.bin", @"G:\temp\FinalVariant\index.bin", md5, log);

            //var ids = File.ReadAllLines(@"G:\temp\testdata.txt");
            //var result = searcher.Search(ids);
            //File.WriteAllLines(@"G:\temp\idsAndHashes.txt", result.Select(x => $"{x.Key}|{x.Value}"));


            foreach (var v in searcher.Search(new[] {"00020000ca27ac810c1a5ff984dc8e69"}))
            {
                System.Console.WriteLine(v);
            }

            foreach (var v in searcher.Search(new[] {"73f07ef50e3d97aa376c56795481c341"}))
            {
                System.Console.WriteLine(v);
            }
        }

        private static void GenerateFiles()
        {
            var log = new Log();
            var md5 = new OptimizedHashProviderFactory();
            var generator = new Uint32ToTiont32AllHashesFileGenerator(md5, log);
            generator.GenerateFile(@"G:\temp\FinalVariant", new ConsoleProgress(log));
        }

        class ConsoleProgress : IProgress<int>
        {
            private readonly ILog _log;

            public ConsoleProgress(ILog log)
            {
                _log = log;
            }
            public void Report(int value)
            {
                _log.Info($"Reporting progress: {value}");
            }
        }
    }
}

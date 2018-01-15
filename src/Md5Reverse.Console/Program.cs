using Md5Reverse.Lib;
using Md5Reverse.Lib.Core;

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

            GenerateAllHashes();
        }


        static void GenerateAllHashes()
        {
            var md5 = new FastUin32HashProvider();
            var log = new Log();
            var folder = @"G:\temp\FinalVariant\";

            var generator = new Uint32ToTiont32AllHashesFileGenerator(md5, log);
            generator.GenerateFile(folder);
        }
    }
}

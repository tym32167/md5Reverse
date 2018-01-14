using Md5Reverse.Lib.Core;
using Md5Reverse.Lib.Utils;

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

        public void GenerateFile(string path)
        {
            using (var bw = FileReaderWriterFactory.CreateBinaryWriter(path, _log))
            {
                _log.Info($"File generation started {path}");

                using (_log.Timing("Generation file"))
                {
                    for (uint i = 0; ; i++)
                    {
                        bw.Write(_md5Provider.ComputeUIntHash(i));

                        if ((i + 1) / 10000000 > i / 10000000)
                        {
                            var percent = i * 100.0 / uint.MaxValue;
                            _log.Info($"Percent progress: {percent}");
                        }

                        if (i == uint.MaxValue) break;
                    }

                    _log.Info($"File generation finished {path}");
                }
            }
        }
    }
}

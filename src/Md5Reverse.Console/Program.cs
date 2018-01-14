using Md5Reverse.Lib;
using Md5Reverse.Lib.Core;
using Md5Reverse.Lib.Utils;

namespace Md5Reverse.Console
{
    public class Program
    {
        static void Main(string[] args)
        {
            ComputeChans();
            //NumDuplicates();
        }

        static void GenerateAllHashes()
        {
            var md5 = new FastUin32HashProvider();
            var log = new Log();
            var fname = @"G:\temp\NextApproach\hashes.bin";
            var generator = new Uint32ToTiont32AllHashesFileGenerator(md5, log);
            generator.GenerateFile(fname);
        }




        static void NumDuplicates()
        {
            var fname = @"G:\temp\NextApproach\chain_hashes.bin";
            var log = new Log();
            var bitarray = new UIntBitArray();
            long numDuplicates = 0;
            long iterations = 0;

            using (log.Timing("NumDuplicates"))
            {
                using (var br = FileReaderWriterFactory.CreateBinaryReader(fname, log))
                {
                    while (br.BaseStream.Position < br.BaseStream.Length)
                    {
                        var hash = br.ReadUInt32();
                        var arrvalue = bitarray.Get(hash);
                        if (arrvalue)
                        {
                            numDuplicates++;
                        }
                        else bitarray.Set(hash, true);
                        iterations++;

                        if (iterations % 10000000 == 0)
                        {
                            var percent = br.BaseStream.Position * 100.0 / br.BaseStream.Length;
                            log.Info($"POS: {br.BaseStream.Position}, percent: {percent}, duplicates: {numDuplicates}, iteration: {iterations}");
                        }
                    }
                }
            }

            log.Info($"Done. Duplicates: {numDuplicates}");
        }


        static void ComputeChans()
        {
            var bitarray = new UIntBitArray();
            var log = new Log();
            var md5 = new FastUin32HashProvider();
            var fname = @"G:\temp\NextApproach\Step1\chain4.bin";

            using (log.Timing("ComputeChans"))
            {
                using (var bw = FileReaderWriterFactory.CreateBinaryWriter(fname, log, 10 * 1024 * 1024))
                {
                    uint processedHashes = 0;
                    uint chains = 0;

                    uint newChains = 0;

                    for (uint i = 0; ; i++)
                    {
                        if (!bitarray.Get(i))
                        {
                            var start = i;
                            var cur = i;

                            uint j = 0;
                            for (; j < 1500; j++)
                            {
                                if (!bitarray.Get(cur))
                                {
                                    processedHashes++;
                                    bitarray.Set(cur, true);
                                }

                                var nextInd = cur + j;

                                if (!bitarray.Get(nextInd))
                                {
                                    processedHashes++;
                                    bitarray.Set(nextInd, true);
                                }

                                var r = md5.ComputeUIntHash(nextInd);
                                cur = md5.ComputeUIntHash(r);
                            }

                            bw.Write(start);
                            bw.Write(cur);

                            chains++;
                            newChains++;
                        }

                        if ((i + 1) / 100000 > i / 100000)
                        {
                            var percent = processedHashes * 100.0 / uint.MaxValue;
                            log.Info($"Percent done: {percent}, current: {i} chains: {chains}, new chains: {newChains}");
                            newChains = 0;
                        }

                        if (i == uint.MaxValue) break;
                    }
                }
            }
        }
    }
}

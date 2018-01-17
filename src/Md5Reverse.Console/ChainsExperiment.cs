using System;
using Md5Reverse.Lib;
using Md5Reverse.Lib.Core;
using Md5Reverse.Lib.Utils;

namespace Md5Reverse.Console
{
    public class ChainsExperiment
    {
        public static void ComputeChanis()
        {
            var bitarray = new UIntBitArray();
            var log = new Log();
            var md5 = new FastUin32HashProvider();
            var fname = @"G:\temp\ChainGen\chain_pure6.bin";

            using (log.Timing(nameof(ComputeChanis)))
            {
                using (var bw = FileReaderWriterFactory.CreateBinaryWriter(fname, log, 10 * 1024 * 1024))
                {
                    uint processedHashes = 0;
                    uint chains = 0;
                    uint newChains = 0;

                    var r = new Random();

                    for (uint j = 0; ; j++)
                    {
                        var i = (uint)r.Next(int.MinValue, int.MaxValue);

                        if (!bitarray.Get(i))
                        {
                            var start = i;
                            var cur = i;

                            for (; ;)
                            {
                                if (bitarray.Get(cur)) break;

                                processedHashes++;
                                bitarray.Set(cur, true);
                                cur = md5.ComputeUIntHash(cur);
                            }

                            bw.Write(start);
                            bw.Write(cur);

                            chains++;
                            newChains++;
                        }

                        if ((j + 1) / 100000 > j / 100000)
                        {
                            var percent = processedHashes * 100.0 / uint.MaxValue;
                            log.Info(
                                $"Percent done: {percent}, current: {j} chains: {chains}, new chains: {newChains}, averageLen: {1.0 * processedHashes / chains}");
                            newChains = 0;
                        }

                        if (j == uint.MaxValue) break;
                    }


                    processedHashes = 0;
                    chains = 0;
                    newChains = 0;

                    for (uint i = 0; ; i++)
                    {
                        if (!bitarray.Get(i))
                        {
                            var start = i;
                            var cur = i;

                            for (;;)
                            {
                                if (bitarray.Get(cur)) break;

                                processedHashes++;
                                bitarray.Set(cur, true);
                                cur = md5.ComputeUIntHash(cur);
                            }

                            bw.Write(start);
                            bw.Write(cur);

                            chains++;
                            newChains++;
                        }

                        if ((i + 1) / 100000 > i / 100000)
                        {
                            var percent = processedHashes * 100.0 / uint.MaxValue;
                            log.Info(
                                $"Percent done: {percent}, current: {i} chains: {chains}, new chains: {newChains}, averageLen: {1.0 * processedHashes / chains}");
                            newChains = 0;
                        }

                        if (i == uint.MaxValue) break;
                    }
                }
            }
        }
    }
}
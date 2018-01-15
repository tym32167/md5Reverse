using Md5Reverse.Lib;
using Md5Reverse.Lib.Core;
using Md5Reverse.Lib.Utils;
using System.Collections.Generic;

namespace Md5Reverse.Console
{
    public class RainbowTableExperiment
    {
        static void ComputeChanis()
        {
            var bitarray = new UIntBitArray();
            var log = new Log();
            var md5 = new FastUin32HashProvider();
            var fname = @"G:\temp\NextApproach\Step1\chain4.bin";

            using (log.Timing(nameof(ComputeChanis)))
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
                            log.Info(
                                $"Percent done: {percent}, current: {i} chains: {chains}, new chains: {newChains}");
                            newChains = 0;
                        }

                        if (i == uint.MaxValue) break;
                    }
                }
            }
        }


        static void TEST()
        {
            var dict = CreateDictFromFile();
            var source = 93611904u;

            var log = new Log();

            using (log.Timing("TEST"))
            {
                var decoded = DecodeHash(source, dict);
                System.Console.WriteLine(decoded);
            }
        }

        static uint DecodeHash(uint hash, Dictionary<uint, List<uint>> table)
        {
            var result = 0u;
            var md5 = new FastUin32HashProvider();

            if (table.ContainsKey(hash))
            {
                var res = FindMatch(hash, table, hash);
                if (res != 0) return res;
            }
            else
            {
                for (uint i = 0; i < 1500; i++)
                {
                    uint j = i;
                    var cur = hash;

                    for (; j < 1500; j++)
                    {
                        var nextInd = cur + j;

                        var r = md5.ComputeUIntHash(nextInd);
                        cur = md5.ComputeUIntHash(r);

                        if (table.ContainsKey(r))
                        {
                            var res = FindMatch(hash, table, r);
                            if (res != 0) return res;
                        }

                        if (table.ContainsKey(cur))
                        {
                            var res = FindMatch(hash, table, cur);
                            if (res != 0) return res;
                        }
                    }
                }
            }

            return result;
        }

        static uint FindMatch(uint hash, Dictionary<uint, List<uint>> table, uint end)
        {
            var md5 = new FastUin32HashProvider();

            foreach (var start in table[end])
            {
                uint j = 0;
                var cur = start;

                for (; j < 1500; j++)
                {
                    var nextInd = cur + j;

                    var r = md5.ComputeUIntHash(nextInd);
                    cur = md5.ComputeUIntHash(r);

                    if (hash == r) return nextInd;
                    if (hash == cur) return r;
                }
            }

            return 0;
        }


        static Dictionary<uint, List<uint>> CreateDictFromFile()
        {
            var dict = new Dictionary<uint, List<uint>>();
            var log = new Log();

            var fname = @"G:\temp\NextApproach\Step1\chain4.bin";

            using (log.Timing("CreateDictFromFile"))
            {
                using (var br = FileReaderWriterFactory.CreateBinaryReader(fname, log, 10 * 1024 * 1024))
                {
                    //while (br.BaseStream.Position < 40 * 1024 * 1024)
                    while (br.BaseStream.Position < br.BaseStream.Length)
                    {
                        var start = br.ReadUInt32();
                        var end = br.ReadUInt32();

                        if (dict.ContainsKey(end))
                        {
                            dict[end].Add(start);
                        }
                        else
                        {
                            var list = new List<uint> { start };
                            dict.Add(end, list);
                        }
                    }
                }
            }

            return dict;
        }
    }
}

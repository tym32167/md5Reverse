using Md5Reverse.Lib.Core;
using Md5Reverse.Lib.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Md5Reverse.Console
{
    public class IdByHashSearcher
    {
        private readonly string _idsFile;
        private readonly string _indexFile;
        private readonly IMd5Provider _md5Provider;
        private readonly ILog _log;

        private Lazy<uint[]> _cache;


        public IdByHashSearcher(string idsFile, string indexFile, IMd5Provider md5Provider, ILog log)
        {
            _idsFile = idsFile;
            _indexFile = indexFile;
            _md5Provider = md5Provider;
            _log = log;

            _cache = new Lazy<uint[]>(() =>
            {
                using (_log.Timing("Creating cache"))
                {
                    return GetCache(_indexFile);
                }
            });
        }

        // 70473 - 6:13 vs 1:15
        public Dictionary<string, long> Search(string[] ids)
        {
            var filteredIds = ids.Where(IsGuid).Distinct().ToArray();

            var idsHash = new HashSet<string>(filteredIds);
            var result = new Dictionary<string, long>();

            _log.Info($"Given ids:{ids.Length}, Filtered ids:{filteredIds.Length}, Not correct ids:{ids.Length - filteredIds.Length}");

            using (_log.Timing($"Searching for {filteredIds.Length} ids"))
            {
                using (var sr = _idsFile.CreateReader().Buffered(4 * 400).ToBinaryReader())
                {
                    foreach (var id in filteredIds)
                    {
                        Search(id, _cache.Value, sr, idsHash, result);
                        if (!idsHash.Any()) break;
                    }
                }
                _log.Info($"Found {result.Count} ids");
            }

            return result;
        }


        //private readonly byte[] _buffer = new byte[4 * 1400];
        long lmin = 0x0110000100000000;


        private long Search(string shash, uint[] cache, BinaryReader reader, HashSet<string> idsHashSet, Dictionary<string, long> result)
        {
            var hash = FromString(shash);
            var ind = hash[0] * 256 * 256 + hash[1] * 256 + hash[2];
            var startLine = cache[ind];
            var endLine = uint.MaxValue;
            if (ind < cache.Length - 1)
                endLine = cache[ind + 1];

            reader.BaseStream.Position = startLine * 4L;

            for (var i = startLine; i < endLine; i++)
            {
                var id = reader.ReadUInt32();
                var ui = id + lmin;
                var midhash = _md5Provider.ComputeByteHash(ui);

                var str = ToString(midhash);

                if (idsHashSet.Contains(str))
                {
                    result.Add(str, ui);
                    idsHashSet.Remove(str);
                }


                //if (CompareTo(hash, midhash) == 0) return ui;
            }

            //var len = reader.Read(_buffer, 0, _buffer.Length);

            //for (var i = 0; i < len; i += 4)
            //{
            //    var ui = BitConverter.ToUInt32(_buffer, i) + lmin;
            //    var midhash = _md5Provider.ComputeByteHash(ui);
            //    if (CompareTo(hash, midhash) == 0) return ui;
            //}

            return 0;
        }

        private static int CompareTo(byte[] s1, byte[] s2)
        {
            for (int i = 0; i < s1.Length; i++)
            {
                if (s1[i] > s2[i]) return 1;
                if (s1[i] < s2[i]) return -1;
            }

            return 0;
        }

        private static bool IsGuid(string str)
        {
            if (string.IsNullOrEmpty(str)) return false;
            if (str.Length != 32) return false;
            foreach (var c in str)
            {
                if (!char.IsLetterOrDigit(c)) return false;
                if (char.IsLetter(c) && char.IsUpper(c)) return false;
            }
            return true;
        }

        public static byte[] FromString(String hex)
        {
            int numberChars = hex.Length;
            byte[] bytes = new byte[numberChars / 2];
            for (int i = 0; i < numberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }

        public static string ToString(byte[] hash)
        {
            var result = "";
            foreach (var b in hash)
                result += b.ToString("x2");

            return result;
        }


        internal static uint[] GetCache(string indexFile)
        {
            var cache = new uint[256 * 256 * 256];

            using (var reader = indexFile.CreateReader().Buffered(10 * 1024 * 1024).ToBinaryReader())
            {
                for (var i = 0; i < 256 * 256 * 256; i++)
                {
                    cache[i] = reader.ReadUInt32();
                }
            }

            return cache;
        }
    }
}
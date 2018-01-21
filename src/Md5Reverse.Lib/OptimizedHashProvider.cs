using Md5Reverse.Lib.Core;
using System;

namespace Md5Reverse.Lib
{
    public class OptimizedHashProvider : IMd5Provider
    {
        private UInt32[] buffer = new uint[4];

        public uint ComputeUIntHash(uint input)
        {
            ComputeByteHash(buffer, input);
            return buffer[0];
        }

        public byte[] ComputeByteHash(uint input)
        {
            ComputeByteHash(buffer, input);
            return BitConverter.GetBytes(buffer[0]);
        }

        long lmin = 0x0110000100000000;
        public byte[] ComputeByteHash(long input)
        {
            var uintInput = (uint)(input - lmin);
            ComputeByteHash(buffer, uintInput);

            var result = new byte[16];
            for (int i = 0; i < 4; i++)
            {
                var bytes = BitConverter.GetBytes(buffer[i]);
                for (int j = 0; j < 4; j++)
                {
                    result[i * 4 + j] = bytes[j];
                }
            }
            return result;
        }



        private static readonly UInt32[] Block = { 0, 0, 0x00800110, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0x50, 0 };
        //              [10]=0x80 [9]=1, [8]=16
        public void ComputeByteHash(UInt32[] state, uint input)
        {
            UInt32 a = state[0] = 0x67452301;
            UInt32 b = state[1] = 0xEFCDAB89;
            UInt32 c = state[2] = 0x98BADCFE;
            UInt32 d = state[3] = 0x10325476;
            UInt32 x;

            Block[0] = 0x4542 | ((input << 16) & 0xFFFF0000);
            Block[1] = 0x00010000 | ((input >> 16) & 0xFFFF);

            x = (a + (d ^ (b & (c ^ d))) + 0xD76AA478 + Block[0]); a = ((x << 7) | (x >> (32 - 7))) + b;
            x = (d + (c ^ (a & (b ^ c))) + 0xE8C7B756 + Block[1]); d = ((x << 12) | (x >> (32 - 12))) + a;
            x = (c + (b ^ (d & (a ^ b))) + 0x242070DB + Block[2]); c = ((x << 17) | (x >> (32 - 17))) + d;
            x = (b + (a ^ (c & (d ^ a))) + 0xC1BDCEEE + Block[3]); b = ((x << 22) | (x >> (32 - 22))) + c;
            x = (a + (d ^ (b & (c ^ d))) + 0xF57C0FAF + Block[4]); a = ((x << 7) | (x >> (32 - 7))) + b;
            x = (d + (c ^ (a & (b ^ c))) + 0x4787C62A + Block[5]); d = ((x << 12) | (x >> (32 - 12))) + a;
            x = (c + (b ^ (d & (a ^ b))) + 0xA8304613 + Block[6]); c = ((x << 17) | (x >> (32 - 17))) + d;
            x = (b + (a ^ (c & (d ^ a))) + 0xFD469501 + Block[7]); b = ((x << 22) | (x >> (32 - 22))) + c;
            x = (a + (d ^ (b & (c ^ d))) + 0x698098D8 + Block[8]); a = ((x << 7) | (x >> (32 - 7))) + b;
            x = (d + (c ^ (a & (b ^ c))) + 0x8B44F7AF + Block[9]); d = ((x << 12) | (x >> (32 - 12))) + a;
            x = (c + (b ^ (d & (a ^ b))) + 0xFFFF5BB1 + Block[10]); c = ((x << 17) | (x >> (32 - 17))) + d;
            x = (b + (a ^ (c & (d ^ a))) + 0x895CD7BE + Block[11]); b = ((x << 22) | (x >> (32 - 22))) + c;
            x = (a + (d ^ (b & (c ^ d))) + 0x6B901122 + Block[12]); a = ((x << 7) | (x >> (32 - 7))) + b;
            x = (d + (c ^ (a & (b ^ c))) + 0xFD987193 + Block[13]); d = ((x << 12) | (x >> (32 - 12))) + a;
            x = (c + (b ^ (d & (a ^ b))) + 0xA679438E + Block[14]); c = ((x << 17) | (x >> (32 - 17))) + d;
            x = (b + (a ^ (c & (d ^ a))) + 0x49B40821 + Block[15]); b = ((x << 22) | (x >> (32 - 22))) + c;
            //Раунд 1
            x = (a + (c ^ (d & (b ^ c))) + 0xF61E2562 + Block[1]); a = ((x << 5) | (x >> (32 - 5))) + b;
            x = (d + (b ^ (c & (a ^ b))) + 0xC040B340 + Block[6]); d = ((x << 9) | (x >> (32 - 9))) + a;
            x = (c + (a ^ (b & (d ^ a))) + 0x265E5A51 + Block[11]); c = ((x << 14) | (x >> (32 - 14))) + d;
            x = (b + (d ^ (a & (c ^ d))) + 0xE9B6C7AA + Block[0]); b = ((x << 20) | (x >> (32 - 20))) + c;
            x = (a + (c ^ (d & (b ^ c))) + 0xD62F105D + Block[5]); a = ((x << 5) | (x >> (32 - 5))) + b;
            x = (d + (b ^ (c & (a ^ b))) + 0x02441453 + Block[10]); d = ((x << 9) | (x >> (32 - 9))) + a;
            x = (c + (a ^ (b & (d ^ a))) + 0xD8A1E681 + Block[15]); c = ((x << 14) | (x >> (32 - 14))) + d;
            x = (b + (d ^ (a & (c ^ d))) + 0xE7D3FBC8 + Block[4]); b = ((x << 20) | (x >> (32 - 20))) + c;
            x = (a + (c ^ (d & (b ^ c))) + 0x21E1CDE6 + Block[9]); a = ((x << 5) | (x >> (32 - 5))) + b;
            x = (d + (b ^ (c & (a ^ b))) + 0xC33707D6 + Block[14]); d = ((x << 9) | (x >> (32 - 9))) + a;
            x = (c + (a ^ (b & (d ^ a))) + 0xF4D50D87 + Block[3]); c = ((x << 14) | (x >> (32 - 14))) + d;
            x = (b + (d ^ (a & (c ^ d))) + 0x455A14ED + Block[8]); b = ((x << 20) | (x >> (32 - 20))) + c;
            x = (a + (c ^ (d & (b ^ c))) + 0xA9E3E905 + Block[13]); a = ((x << 5) | (x >> (32 - 5))) + b;
            x = (d + (b ^ (c & (a ^ b))) + 0xFCEFA3F8 + Block[2]); d = ((x << 9) | (x >> (32 - 9))) + a;
            x = (c + (a ^ (b & (d ^ a))) + 0x676F02D9 + Block[7]); c = ((x << 14) | (x >> (32 - 14))) + d;
            x = (b + (d ^ (a & (c ^ d))) + 0x8D2A4C8A + Block[12]); b = ((x << 20) | (x >> (32 - 20))) + c;
            //Раунд 2
            x = (a + (b ^ c ^ d) + 0xFFFA3942 + Block[5]); a = ((x << 4) | (x >> (32 - 4))) + b;
            x = (d + (a ^ b ^ c) + 0x8771F681 + Block[8]); d = ((x << 11) | (x >> (32 - 11))) + a;
            x = (c + (d ^ a ^ b) + 0x6D9D6122 + Block[11]); c = ((x << 16) | (x >> (32 - 16))) + d;
            x = (b + (c ^ d ^ a) + 0xFDE5380C + Block[14]); b = ((x << 23) | (x >> (32 - 23))) + c;
            x = (a + (b ^ c ^ d) + 0xA4BEEA44 + Block[1]); a = ((x << 4) | (x >> (32 - 4))) + b;
            x = (d + (a ^ b ^ c) + 0x4BDECFA9 + Block[4]); d = ((x << 11) | (x >> (32 - 11))) + a;
            x = (c + (d ^ a ^ b) + 0xF6BB4B60 + Block[7]); c = ((x << 16) | (x >> (32 - 16))) + d;
            x = (b + (c ^ d ^ a) + 0xBEBFBC70 + Block[10]); b = ((x << 23) | (x >> (32 - 23))) + c;
            x = (a + (b ^ c ^ d) + 0x289B7EC6 + Block[13]); a = ((x << 4) | (x >> (32 - 4))) + b;
            x = (d + (a ^ b ^ c) + 0xEAA127FA + Block[0]); d = ((x << 11) | (x >> (32 - 11))) + a;
            x = (c + (d ^ a ^ b) + 0xD4EF3085 + Block[3]); c = ((x << 16) | (x >> (32 - 16))) + d;
            x = (b + (c ^ d ^ a) + 0x04881D05 + Block[6]); b = ((x << 23) | (x >> (32 - 23))) + c;
            x = (a + (b ^ c ^ d) + 0xD9D4D039 + Block[9]); a = ((x << 4) | (x >> (32 - 4))) + b;
            x = (d + (a ^ b ^ c) + 0xE6DB99E5 + Block[12]); d = ((x << 11) | (x >> (32 - 11))) + a;
            x = (c + (d ^ a ^ b) + 0x1FA27CF8 + Block[15]); c = ((x << 16) | (x >> (32 - 16))) + d;
            x = (b + (c ^ d ^ a) + 0xC4AC5665 + Block[2]); b = ((x << 23) | (x >> (32 - 23))) + c;
            //Раунд 3
            x = (a + (c ^ (b | ~d)) + 0xF4292244 + Block[0]); a = ((x << 6) | (x >> (32 - 6))) + b;
            x = (d + (b ^ (a | ~c)) + 0x432AFF97 + Block[7]); d = ((x << 10) | (x >> (32 - 10))) + a;
            x = (c + (a ^ (d | ~b)) + 0xAB9423A7 + Block[14]); c = ((x << 15) | (x >> (32 - 15))) + d;
            x = (b + (d ^ (c | ~a)) + 0xFC93A039 + Block[5]); b = ((x << 21) | (x >> (32 - 21))) + c;
            x = (a + (c ^ (b | ~d)) + 0x655B59C3 + Block[12]); a = ((x << 6) | (x >> (32 - 6))) + b;
            x = (d + (b ^ (a | ~c)) + 0x8F0CCC92 + Block[3]); d = ((x << 10) | (x >> (32 - 10))) + a;
            x = (c + (a ^ (d | ~b)) + 0xFFEFF47D + Block[10]); c = ((x << 15) | (x >> (32 - 15))) + d;
            x = (b + (d ^ (c | ~a)) + 0x85845DD1 + Block[1]); b = ((x << 21) | (x >> (32 - 21))) + c;
            x = (a + (c ^ (b | ~d)) + 0x6FA87E4F + Block[8]); a = ((x << 6) | (x >> (32 - 6))) + b;
            x = (d + (b ^ (a | ~c)) + 0xFE2CE6E0 + Block[15]); d = ((x << 10) | (x >> (32 - 10))) + a;
            x = (c + (a ^ (d | ~b)) + 0xA3014314 + Block[6]); c = ((x << 15) | (x >> (32 - 15))) + d;
            x = (b + (d ^ (c | ~a)) + 0x4E0811A1 + Block[13]); b = ((x << 21) | (x >> (32 - 21))) + c;
            x = (a + (c ^ (b | ~d)) + 0xF7537E82 + Block[4]); a = ((x << 6) | (x >> (32 - 6))) + b;
            x = (d + (b ^ (a | ~c)) + 0xBD3AF235 + Block[11]); d = ((x << 10) | (x >> (32 - 10))) + a;
            x = (c + (a ^ (d | ~b)) + 0x2AD7D2BB + Block[2]); c = ((x << 15) | (x >> (32 - 15))) + d;
            x = (b + (d ^ (c | ~a)) + 0xEB86D391 + Block[9]); b = ((x << 21) | (x >> (32 - 21))) + c;

            state[0] = state[0] + a;
            state[1] = state[1] + b;
            state[2] = state[2] + c;
            state[3] = state[3] + d;

        }
    }
}
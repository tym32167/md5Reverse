using System;
using System.Collections;

namespace Md5Reverse.Lib.Utils
{
    public class UIntBitArray
    {
        private readonly BitArray[] _arrays = { new BitArray(Int32.MaxValue), new BitArray(Int32.MaxValue), new BitArray(10) };

        public void Set(uint ind, bool value)
        {
            var arrInd = ind / Int32.MaxValue;
            var realind = ind % Int32.MaxValue;

            //Console.WriteLine($"SET arrInd: {arrInd}, realind: {realind}");
            _arrays[arrInd].Set((int)realind, value);
        }

        public bool Get(uint ind)
        {
            var arrInd = ind / Int32.MaxValue;
            var realind = ind % Int32.MaxValue;

            //Console.WriteLine($"GET arrInd: {arrInd}, realind: {realind}");
            return _arrays[arrInd].Get((int)realind);
        }
    }
}

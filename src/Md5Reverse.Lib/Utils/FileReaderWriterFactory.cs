using System.IO;
using Md5Reverse.Lib.Core;

namespace Md5Reverse.Lib.Utils
{
    public static class FileReaderWriterFactory
    {
        private const int BufferLen = 100 * 1024 * 1024;

        public static BinaryReader CreateBinaryReader(string path, ILog log, int bufferLen = BufferLen)
        {
            return new BinaryReader(new BufferedStream(
                new StreamSpy(
                    File.OpenRead(path), log), bufferLen));
        }

        public static BinaryWriter CreateBinaryWriter(string path, ILog log, int bufferLen = BufferLen)
        {
            return new BinaryWriter(new BufferedStream(
                new StreamSpy(
                    File.OpenWrite(path), log), bufferLen));
        }
    }
}

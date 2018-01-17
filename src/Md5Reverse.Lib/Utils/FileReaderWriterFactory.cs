using Md5Reverse.Lib.Core;
using System.IO;

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


        public static StreamSpy WithSpy(this Stream source, ILog log)
        {
            return new StreamSpy(source, log);
        }

        public static BufferedStream Buffered(this Stream source, int bufferLen = BufferLen)
        {
            return new BufferedStream(source, bufferLen);
        }

        public static BinaryWriter ToBinaryWriter(this Stream stream)
        {
            return new BinaryWriter(stream);
        }

        public static BinaryReader ToBinaryReader(this Stream stream)
        {
            return new BinaryReader(stream);
        }

        public static FileStream CreateReader(this string src)
        {
            return File.OpenRead(src);
        }

        public static FileStream CreateWriter(this string src)
        {
            return File.OpenWrite(src);
        }
    }
}

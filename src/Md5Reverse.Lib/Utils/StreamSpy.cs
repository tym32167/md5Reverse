﻿using Md5Reverse.Lib.Core;
using System.IO;

namespace Md5Reverse.Lib.Utils
{
    public class StreamSpy : Stream
    {
        private readonly Stream _inner;
        private readonly ILog _log;

        public StreamSpy(Stream inner, ILog log)
        {
            _inner = inner;
            _log = log;
        }

        public override void Flush()
        {
            _inner.Flush();
            _log.Debug("Flush!");
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return _inner.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            _inner.SetLength(value);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            _log.Debug($"READ buffer {buffer.Length}, offset {offset}, count {count}, POSITION: {Position}");
            return _inner.Read(buffer, offset, count);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            _inner.Write(buffer, offset, count);
            _log.Debug($"WRITE buffer {buffer.Length}, offset {offset}, count {count}");
        }

        public override bool CanRead => _inner.CanRead;
        public override bool CanSeek => _inner.CanSeek;
        public override bool CanWrite => _inner.CanWrite;
        public override long Length => _inner.Length;
        public override long Position
        {
            get => _inner.Position;
            set => _inner.Position = value;
        }

        public override void Close()
        {
            base.Close();
            _inner.Close();

            _log.Info("CLOSE");
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            _inner.Dispose();
            _log.Info("DISPOSE");
        }
    }
}
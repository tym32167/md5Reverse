using System;
using System.Diagnostics;

namespace Md5Reverse.Lib.Core
{
    public interface ILog
    {
        void Debug(object message);
        void Info(object message);
        void Error(object message);
        void Fatal(object message);
        IDisposable Timing(string text);
    }

    public class Log : ILog
    {
        public void Debug(object message)
        {
//#if DEBUG

            System.Console.WriteLine($"{DateTime.UtcNow:T} Debug: {message}");
//#endif
        }

        public void Info(object message)
        {
            System.Console.WriteLine($"{DateTime.UtcNow:T} Info: {message}");
        }

        public void Error(object message)
        {
            System.Console.WriteLine($"{DateTime.UtcNow:T} Error: {message}");
        }

        public void Fatal(object message)
        {
            System.Console.WriteLine($"{DateTime.UtcNow:T} Fatal: {message}");
        }

        public IDisposable Timing(string text)
        {
            return new TimingHelper(text, this);
        }


        private class TimingHelper : IDisposable
        {
            private readonly string _text;
            private readonly ILog _log;
            private readonly Stopwatch _watch = new Stopwatch();

            public TimingHelper(string text, ILog log)
            {
                _text = text;
                _log = log;

                _log.Info($"{text} started.");

                _watch.Start();
            }

            public void Dispose()
            {
                _watch.Stop();

                _log.Info($"{_text} finished. Timing: {_watch.Elapsed}.");
            }
        }
    }
}
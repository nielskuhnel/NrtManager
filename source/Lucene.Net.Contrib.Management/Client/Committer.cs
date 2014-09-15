using System;
using System.Diagnostics;
using System.Threading;
using Lucene.Net.Index;

namespace Lucene.Net.Contrib.Management.Client
{
    public class Committer : IDisposable
    {
        private readonly IndexWriter _writer;
        private readonly TimeSpan _commitInterval;
        private readonly TimeSpan _optimizeInterval;
        private readonly AutoResetEvent _waitHandle = new AutoResetEvent(false);
        private bool _finish;

        public Committer(IndexWriter writer, TimeSpan commitInterval, TimeSpan optimizeInterval)
        {
            _writer = writer;
            _commitInterval = commitInterval;
            _optimizeInterval = optimizeInterval;
        }

        public void Start()
        {
            var sw = new Stopwatch();
            var lastOptimize = 0L;
            sw.Start();
            while (!_finish)
            {
                _writer.Commit();

                if (sw.ElapsedTicks - lastOptimize > _optimizeInterval.Ticks)
                {
                    _writer.Optimize(2, false);
                    lastOptimize = sw.ElapsedTicks;
                }

                _waitHandle.WaitOne(_commitInterval);
            }
            sw.Stop();
        }

        public void Dispose()
        {
            _finish = true;
            _waitHandle.Set();
        }
    }
}
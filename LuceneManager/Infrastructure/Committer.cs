using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Web;
using Lucene.Net.Index;

namespace LuceneManager.Infrastructure
{
    public class Committer : IDisposable
    {
        private readonly IndexWriter _writer;
        private readonly TimeSpan _commitInterval;
        private readonly TimeSpan _optimizeInterval;
        private ManualResetEventSlim _waitHandle = new ManualResetEventSlim(false);
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
                _waitHandle.Reset();

                _writer.Commit();

                if (sw.ElapsedTicks - lastOptimize > _optimizeInterval.Ticks)
                {
                    _writer.Optimize(2, false);
                    lastOptimize = sw.ElapsedTicks;
                }

                _waitHandle.Wait(_commitInterval);
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
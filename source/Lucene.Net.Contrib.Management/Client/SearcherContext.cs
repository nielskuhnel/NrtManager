using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Lucene.Net.Analysis;
using Lucene.Net.Index;
using Lucene.Net.Store;

namespace Lucene.Net.Contrib.Management.Client
{
    public class SearcherContext : IDisposable
    {
        public NrtManager Manager { get; private set; }

        public PerFieldAnalyzerWrapper Analyzer { get; private set; }

        private readonly IndexWriter _writer;

        private readonly NrtManagerReopener _reopener;
        private readonly Committer _committer;

        private readonly List<Thread> _threads = new List<Thread>();

        public SearcherContext(Directory dir, Analyzer defaultAnalyzer)
            : this(dir, defaultAnalyzer, TimeSpan.FromSeconds(.1), TimeSpan.FromSeconds(2),
            TimeSpan.FromSeconds(10), TimeSpan.FromHours(2))
        {
        }

        public SearcherContext(Directory dir, Analyzer defaultAnalyzer,
                        TimeSpan targetMinStale, TimeSpan targetMaxStale,
                        TimeSpan commitInterval, TimeSpan optimizeInterval)
        {
            Analyzer = new PerFieldAnalyzerWrapper(defaultAnalyzer);
            _writer = new IndexWriter(dir, Analyzer, IndexWriter.MaxFieldLength.UNLIMITED);

            Manager = new NrtManager(_writer);
            _reopener = new NrtManagerReopener(Manager, targetMaxStale, targetMinStale);
            _committer = new Committer(_writer, commitInterval, optimizeInterval);

            _threads.AddRange(new[] { new Thread(_reopener.Start), new Thread(_committer.Start) });

            foreach (var t in _threads)
            {
                t.Start();
            }
        }

        public SearcherManager.IndexSearcherToken GetSearcher()
        {
            return Manager.GetSearcherManager().Acquire();
        }

        public void Dispose()
        {
            var disposeActions = new List<Action>
                {
                    _reopener.Dispose,
                    _committer.Dispose,
                    Manager.Dispose,
                    () => _writer.Dispose(true)
                };

            disposeActions.AddRange(_threads.Select(t => (Action)t.Join));

            DisposeUtil.PostponeExceptions(disposeActions.ToArray());
        }
    }
}
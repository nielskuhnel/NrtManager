using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;

namespace LuceneManager.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index(string q = null)
        {
            var model = new List<string>();
            using (var s = MvcApplication.Searcher.GetSearcher())
            {
                var query = string.IsNullOrWhiteSpace(q) ? (Query) new MatchAllDocsQuery() : new TermQuery(new Term("Text", q));

                var c = TopScoreDocCollector.Create(100, true);

                s.Searcher.Search(query, c);

                model.AddRange(c.TopDocs().ScoreDocs.Select(d => s.Searcher.Doc(d.Doc).Get("Text")));

                return View(model);
            }
        }


        public ActionResult Save(string text, bool wait = false)
        {
            var doc = new Document();
            doc.Add(new Field("Text", text, Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.WITH_POSITIONS_OFFSETS));
            var gen = MvcApplication.Searcher.Manager.AddDocument(doc);

            if (wait)
            {
                MvcApplication.Searcher.Manager.WaitForGeneration(gen);
            }

            return RedirectToAction("Index");
        }
    }
}

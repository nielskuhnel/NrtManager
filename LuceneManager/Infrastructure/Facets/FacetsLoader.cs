using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Lucene.Net.Contrib.Management;
using Lucene.Net.Search;

namespace LuceneManager.Infrastructure.Facets
{
    public class FacetsLoader : ISearcherWarmer
    {
        public void Warm(IndexSearcher s)
        {
            
        }
    }
}
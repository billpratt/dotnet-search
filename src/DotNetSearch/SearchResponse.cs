using System.Collections.Generic;

namespace DotNetSearch
{
    public class SearchResponse
    {
        public int TotalHits { get; set; }
        public IEnumerable<Package> Data { get; set; }
    }
}
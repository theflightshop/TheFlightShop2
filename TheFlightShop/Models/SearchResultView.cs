using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TheFlightShop.Models
{
    public class SearchResultView
    {
        public string Query { get; set; }
        public IEnumerable<SearchResult> Results { get; set; }
    }
}

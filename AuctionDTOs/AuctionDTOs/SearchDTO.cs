using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuctionDTOs
{
    public class SearchDTO
    {
        public string SearchBox { get; set; }
        public string Category { get; set; }
    }

    public class AdvancedSearchDTO : SearchDTO 
    { 
        public string MinimumPrice { get; set; } 
        public string MaximumPrice { get; set; } 
        public DateTime AuctionEndTime { get; set; } 
    }
}

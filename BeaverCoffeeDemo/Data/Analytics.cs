using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeaverCoffeeDemo.Data
{
    public class SalesAnalytics
    {
        public ObjectId ProductId { get; set; }
        public Totals Totals { get; set; }
    }

    public class Totals
    {
        public decimal TotalPrice { get; set; }
        public decimal TotalUnit { get; set; }
    }
}

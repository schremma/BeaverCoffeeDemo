using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeaverCoffeeDemo.Data
{
    public class Stock
    {
        public ObjectId Id { get; set; }
        public string StockName { get; set; }
        public ObjectId LocationId { get; set; }
    }
}

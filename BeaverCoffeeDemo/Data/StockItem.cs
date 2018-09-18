using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeaverCoffeeDemo.Data
{
    public class StockItem
    {
        public ObjectId Id { get; set; }
        public ObjectId StockId { get; set; }
        public ObjectId ProductId { get; set; }
        public double Amount { get; set; }
        public string Unit { get; set; }
    }
}

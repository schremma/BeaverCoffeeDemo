using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeaverCoffeeDemo.Data
{
    public class StockTransaction
    {
        public ObjectId Id { get; set; }
        public ObjectId StockId { get; set; }
        public DateTime Day { get; set; }
        public ObjectId LocationId { get; set; }
        public IEnumerable<TransactionItem> Transactions { get; set; }
    }

    public class TransactionItem
    {
        public ObjectId ProductId { get; set; }
        public double StartingStockAmount { get; set; }
        public double QuantityChange { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}

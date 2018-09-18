using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeaverCoffeeDemo.Data
{
    public class Shop
    {
        public ObjectId Id { get; set; }
        public Address Address { get; set; }
        public ObjectId StockId { get; set; }
    }
}

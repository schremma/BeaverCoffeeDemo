using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeaverCoffeeDemo.Data
{
    public class ProductRange
    {
        public ObjectId Id { get; set; }
        public ObjectId LocationId { get; set; }
        public IEnumerable<ProductRangeItem> Products {get; set; }
}
}

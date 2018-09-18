using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeaverCoffeeDemo.Data
{
    public class ProductRangeItem
    {
        public ObjectId Id { get; set; }
        public String Name { get; set; }
        public Decimal Price { get; set; }
        [BsonRepresentation(BsonType.String)]
        public ProductCategory Category { get; set; }

        public Ingredients Ingredients { get; set; }
    }
}

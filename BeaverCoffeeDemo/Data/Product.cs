using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeaverCoffeeDemo.Data
{
    public class Product
    {
        public ObjectId Id { get; private set; }
        public String EngName { get; set; }
        [BsonRepresentation(BsonType.String)]
        public ProductCategory Category { get; set; }
        public Ingredients Ingredients { get; set; }
    }

    public class Ingredients
    {
        public IEnumerable<BaseIngredient> BaseIngredients { get; set; }
        public MilkTypeIngredients MilkTypeIngredients { get; set; }
        public IEnumerable<ObjectId> ExtraIngredients { get; set; }
    }

    public class BaseIngredient
    {
        public ObjectId Id { get; set; }
        public int StockUnit { get; set; }
    }

    public class MilkTypeIngredients
    {
        public int StockUnit { get; set; }
        public IEnumerable<ObjectId> Options { get; set; }
    }



}

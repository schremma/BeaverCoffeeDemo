using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeaverCoffeeDemo.Data
{
    public class Location
    {
        public ObjectId Id { get; set; }
        public String Name { get; set; }
        public String PersonalIdFormat { get; set; }
        [BsonRepresentation(BsonType.String)]
        public LanguageCode LanguageCode { get; set; }
        [BsonRepresentation(BsonType.String)]
        public Currency Currency { get; set; }
        public IEnumerable<Shop> Shops { get; set; }
    }

    public enum LanguageCode {
        Swe,
        Eng
    }

    public enum Currency {
        SEK,
        USD
    }

}

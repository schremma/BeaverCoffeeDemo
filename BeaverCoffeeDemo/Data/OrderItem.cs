using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeaverCoffeeDemo.Data
{
    public class OrderItem 
    {
        public ObjectId Id { get; set; }
        public Decimal Price { get; set; }
        public int UnitAmount { get; set; }
        public List<ObjectId> AddOnIds { get; set; }
        public ObjectId MilkTypeChoiceId { get; set; }
    }


}

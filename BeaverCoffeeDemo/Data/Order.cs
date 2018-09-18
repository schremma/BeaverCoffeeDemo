using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeaverCoffeeDemo.Data
{
    public class Order
    {
        public ObjectId Id { get; set; }
        public decimal Total { get; set; }
        public ObjectId CustomerId { get; set; }
        public ObjectId ShopId { get; set; }
        public ObjectId RegisteringEmployeeId { get; set; }
        public ObjectId LocationId { get; set; }
        public DateTime OrderDate { get; set; }
        public bool CustomerIsEmployee { get; set; }
        public IEnumerable<OrderItem> OrderItems { get; set; }

        public OrderCustomer Customer { get; set; }

    }

    public class OrderCustomer
    {
        public string Name { get; set; }
        public Address Address { get; set; }
        public Occupation Occupation { get; set; }

        public override string ToString()
        {
            return string.Format("Name: {0}, Occupation: {1}", Name, Occupation);
        }
    }

}

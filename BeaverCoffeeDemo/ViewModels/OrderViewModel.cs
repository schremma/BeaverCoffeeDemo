using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeaverCoffeeDemo.ViewModels
{
    public class OrderViewModel
    {
        public ObjectId OrderId { get; set; }
        public DateTime Date { get; set; }
        public string TotalString { get; set; }
        public decimal Total { get; set; }
        public string ProductDetails { get; set; }
        public string CustomerDetails { get; set; }
    }
}

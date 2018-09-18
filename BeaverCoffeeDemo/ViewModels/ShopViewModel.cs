using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeaverCoffeeDemo.ViewModels
{
    public class ShopViewModel
    {
        public ObjectId Id { get; set; }
        public string ShopDescription { get; set; }
        public AddressViewModel ShopAddress { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeaverCoffeeDemo.Data
{
    public class Address
    {
        public string City { get; set; }
        public string Street { get; set; }
        public string Zip { get; set; }
        public string Country { get; set; }

        public override string ToString()
        {
            return String.Format("{0} {1}, {2}", Street, City, Country);
        }
    }
}

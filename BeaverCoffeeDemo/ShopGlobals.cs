using BeaverCoffeeDemo.Data;
using BeaverCoffeeDemo.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeaverCoffeeDemo
{
    public class ShopGlobals
    {
       
        public static Location Location { get; set; }
        public static ShopViewModel Shop { get; set; }
        public static Employee Employee { get; set; }
        public static ProductRange LocalProductRange { get; set; }
        public static Currency LocalCurrency { get; set; }
    }
}

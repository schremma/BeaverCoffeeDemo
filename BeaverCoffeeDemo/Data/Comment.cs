using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeaverCoffeeDemo.Data
{
    public class Comment
    {
        public ObjectId EmployerId { get; set; }
        public string EmployerName { get; set; }
        public DateTime Date { get; set; }
        public string Text { get; set; }
    }
}

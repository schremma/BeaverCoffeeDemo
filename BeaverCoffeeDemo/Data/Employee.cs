using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeaverCoffeeDemo.Data
{
    public class Employee
    {
        public ObjectId Id { get; set; }
        public string Name { get; set; }
        public string PersonalId { get; set; }
        public DateTime HireDate { get; set; }
        public DateTime EmployementEndDate { get; set; }
        public Address Address { get; set; }

        public IEnumerable<EmployementPosition> Positions { get; set; }
        public ICollection<Comment> Comments { get; set; }
    }

    public class EmployementPosition
    {
        public ObjectId Id { get; set; }
        public int Position { get; set; }
        public ObjectId WorkLocationId { get; set; }
        public ObjectId ShopId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public float FullTimeProcent { get; set; }
        public IEnumerable<ObjectId> BossOver { get; set; }
    }
}

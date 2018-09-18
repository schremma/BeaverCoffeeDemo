using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeaverCoffeeDemo.Data
{
    public class Customer
    {
        public ObjectId Id { get; set; }
        public string Name { get; set; }
        public string PersonalId { get; set; }
        public ObjectId LocationId { get; set; }
        public DateTime RegisterDate { get; set; }
        public DateTime DiscontinuedDate { get; set; }
        public ObjectId RegisteringEmployeeId { get; set; }
        public Address Address { get; set; }
        public int BeverageCounter { get; set; }
        public Occupation Occupation { get; set; }

        public override string ToString()
        {
            return string.Format("Name: {0}, Occupation: {1}", Name, Occupation);
        }
    }

    public enum Occupation
    {
        Student,
        Pensioner,
        Academy,
        Clerical,
        Service,
        Skilled_worker,
        Armed_forces,
        Manager,
        Trades,
        Other
    }
}

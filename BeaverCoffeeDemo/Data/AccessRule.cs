using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeaverCoffeeDemo.Data
{
    public class AccessRule
    {
        public ObjectId Id { get; set; }
        public int Position { get; set; }
        public IEnumerable<UserRole> UserRoles { get; set; }
    }

    public enum UserRole
    {
        ProcessOrder,
        RegisterCustomer,
        ReadWriteEmployee,
        ReadWriteCustomer,
        ReadWriteStock,
        CreateLocationReports,
        CreateGlobalReports
    }
}

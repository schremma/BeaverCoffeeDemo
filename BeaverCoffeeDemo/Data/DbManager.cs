using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver.Linq;
using MongoDB.Driver;
using MongoDB.Bson;

namespace BeaverCoffeeDemo.Data
{
    /// <summary>
    /// Manages all interaction with a MongoDb instance, at localhost and port 27017.
    /// </summary>
    public class DbManager
    {
        private static DbManager dbConnectInstance;
        protected static IMongoClient client;
        protected static IMongoDatabase db;

        private DbManager()
        {
           
            // client with a default localhost and port #27017
            client = new MongoClient();

            db = client.GetDatabase("coffeeshop");

        }

        public static DbManager GetInstance()
        {
            if (dbConnectInstance == null)
                dbConnectInstance = new DbManager();
            return dbConnectInstance;
        }

        /// <summary>
        /// If an operation fails, a specific error message may be set here.
        /// </summary>
        public string Error { get; set; }
        
        /// <summary>
        /// Gets all country locations where BeaverCoffee AB is present (Sweden, US)
        /// </summary>
        public IEnumerable<Location> GetAllLocations()
        {
            var coll = db.GetCollection<Location>("locations");

            var locations = coll.Find(_ => true);
            return locations.ToEnumerable();

        }

        public IEnumerable<Employee> GetAllEmployees()
        {
            var coll = db.GetCollection<Employee>("employees");

            var employees = coll.Find(_ => true);
            return employees.ToEnumerable();

        }

        /// <summary>
        /// Returns a Location object for a particular name.
        /// </summary>
        /// <param name="locationName">The identifying name of the location, e.g. "US"</param>
        /// <returns>Location object or null if namde does not exist in database</returns>
        public Location GetLocationForName(string locationName)
        {
            var coll = db.GetCollection<Location>("locations");
            return coll.AsQueryable().Where(l => l.Name == locationName).FirstOrDefault();
        }

        /// <summary>
        /// Gets the name used to identify a location with a specific id in the database.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Name of the location or empty string if id does not exist</returns>
        public string GetNameOfLocation(ObjectId id)
        {
            if (id != null && id != ObjectId.Empty)
            {
                var coll = db.GetCollection<Location>("locations");
                return coll.AsQueryable().Where(l => l.Id == id).FirstOrDefault().Name;
            }
            return string.Empty;
        }

        /// <summary>
        /// Gets employee by id in the database, if exists.
        /// </summary>
        public Employee GetEmployee(ObjectId employeeId)
        {
            var collection = db.GetCollection<Employee>("employees");
            return collection.AsQueryable().Where(l => l.Id == employeeId).SingleOrDefault();
        }

        /// <summary>
        /// Gets all employees who are holding or have ever held an employement position at the specified country location.
        /// </summary>
        public IEnumerable<Employee> GetEmployeesForLocation(Location location)
        {
            var collection = db.GetCollection<Employee>("employees");
            var employees = collection.AsQueryable().Where(e => e.Positions.Any(p => p.WorkLocationId == location.Id));
            return employees.ToEnumerable();
        }

        /// <summary>
        /// Gets all employees who are currently holding an employement position at the specifiedc country location.
        /// </summary>
        public IEnumerable<Employee> GetCurrentEmployeesForLocation(Location location)
        {
            var collection = db.GetCollection<Employee>("employees");
            var employees = collection.AsQueryable().Where(e => e.Positions.Any(p => p.WorkLocationId == location.Id && p.EndDate < new DateTime(100, 01, 01, 00, 00, 00)));
            return employees.ToEnumerable();
        }

        /// <summary>
        /// Gets all employees who are currently holding a location manager position at the specifiedc country location.
        /// </summary>
        public IEnumerable<Employee> GetCurrentLocationManagers(Location location)
        {
            var accessRules = GetAccessRules();
 
            var positions = accessRules.Where(a => a.UserRoles.Any(r => r.Equals(UserRole.ReadWriteEmployee)) && !a.UserRoles.Any(r => r.Equals(UserRole.CreateGlobalReports))).Select(p => p.Position);

            var collection = db.GetCollection<Employee>("employees");
            var employees = collection.AsQueryable().Where(e => e.Positions.Any(p => p.WorkLocationId == location.Id
                && positions.Contains(p.Position) && p.EndDate < new DateTime(100, 01, 01, 00, 00, 00)));
            return employees.ToEnumerable();
        }

        /// <summary>
        /// Gets all employees who are currently holding a corporate manager position at any location.
        /// </summary>
        public IEnumerable<Employee> GetCurrentCorporateManagers()
        {
            var accessRules = GetAccessRules();

            var positions = accessRules.Where(a => a.UserRoles.Any(r => r.Equals(UserRole.CreateGlobalReports))).Select(p => p.Position);

            var collection = db.GetCollection<Employee>("employees");
            var employees = collection.AsQueryable().Where(e => e.Positions.Any(p => positions.Contains(p.Position) && p.EndDate < new DateTime(100, 01, 01, 00, 00, 00)));
            return employees.ToEnumerable();
        }

        /// <summary>
        /// Get all regular shop employees, serving customers, at the specified country location.
        /// </summary>
        public IEnumerable<Employee> GetShopEmployeesForLocation(Location location)
        {
            var collection = db.GetCollection<Employee>("employees");
            var employees = collection.AsQueryable().Where(e => e.Positions.Any(p => p.WorkLocationId == location.Id && p.ShopId != ObjectId.Empty));
            return employees.ToEnumerable();
        }


        /// <summary>
        /// Get all regular shop employees, serving customers, at any location.
        /// </summary>
        public IEnumerable<Employee> GetAllShopEmployees()
        {
            var collection = db.GetCollection<Employee>("employees");
            var employees = collection.AsQueryable().Where(e => e.Positions.Any(p => p.ShopId != ObjectId.Empty));
            return employees.ToEnumerable();
        }

        /// <summary>
        /// Get all shop employees, i.e. employees serving customers, who are currently working at the specified coffee shop.
        /// </summary>
        public IEnumerable<Employee> GetCurrentEmployeesForShop(ObjectId shopId)
        {
            var collection = db.GetCollection<Employee>("employees");
            var employees = collection.AsQueryable().Where(e => e.Positions.Any(p => p.ShopId == shopId && p.EndDate < new DateTime(100, 01, 01, 00, 00, 00)));
            return employees.ToEnumerable();
        }

        /// <summary>
        /// Get all shop employees, serving customers, who has ever worked at the specified coffee shop.
        /// </summary>
        public IEnumerable<Employee> GetEmployeesForShop(ObjectId shopId)
        {
            var collection = db.GetCollection<Employee>("employees");
            var employees = collection.AsQueryable().Where(e => e.Positions.Any(p => p.ShopId == shopId && p.EndDate < new DateTime(100, 01, 01, 00, 00, 00)));
            return employees.ToEnumerable();
        }

        public Address GetShopAddressForId(ObjectId shopId)
        {
            var collection = db.GetCollection<Location>("locations");

             var shop = collection.AsQueryable().SelectMany(l => l.Shops).FirstOrDefault(s => s.Id == shopId);

            return shop != null ? shop.Address : null;
            
        }

        /// <summary>
        /// Updates an already existing employee with the information stored in the input parameter.
        /// </summary>
        /// <param name="emp">Employee with updated data</param>
        /// <returns>True, if a successful update took place. False if update failed or no update was performed</returns>
        public bool UpdateEmployee(Employee emp)
        {
            var collection = db.GetCollection<Employee>("employees");
            var filter = Builders<Employee>.Filter.Eq("Id", emp.Id);
            var result = collection.ReplaceOne(filter, emp);
            return result.IsAcknowledged && result.ModifiedCount >= 1;
        }

        /// <summary>
        /// Updates one specific field of an employee document in the database.
        /// </summary>
        /// <typeparam name="T">The type of the porerty to be updated</typeparam>
        /// <param name="property">The new value of the property</param>
        /// <param name="propertyName">The name of the property in Employee object to be updated</param>
        /// <param name="employeeId">The id of the employee to be updated</param>
        /// <returns>True, if a successful update took place. False if update failed or no update was performed</returns>
        public bool UpdateEmployeeProperty<T>(T property, string propertyName, ObjectId employeeId)
        {
            var collection = db.GetCollection<Employee>("employees");
            var filter = Builders<Employee>.Filter.Eq("Id", employeeId);

            var update = Builders<Employee>.Update.Set(propertyName, property);
            var result = collection.UpdateOne(filter, update);
            return result.IsAcknowledged && result.ModifiedCount >= 1;
        }

        public bool UpdatePPositionProperty<T>(T property, string propertyName, ObjectId positionId)
        {
            var collection = db.GetCollection<Employee>("employees");
            var filter = Builders<Employee>.Filter.Eq("Positions._id", positionId);

            string fullPropertyName = "Positions.$." + propertyName;
            var update = Builders<Employee>.Update.Set(fullPropertyName, property);
            var result = collection.UpdateOne(filter, update);
            return result.IsAcknowledged && result.ModifiedCount >= 1;
        }

        /// <summary>
        /// Updates the coffee shop where a particular employee works at in a specific employement position.
        /// </summary>
        /// <param name="pos">The EmployementPosition associated with the coffee shop</param>
        /// <param name="employeeId">The id of the employee holding the employement position to be updated</param>
        /// <returns>True, if a successful update took place. False if update failed or no update was performed</returns>
        public bool UpdatePositionShopIdOfEmployee(EmployementPosition pos, ObjectId employeeId)
        {
            var collection = db.GetCollection<Employee>("employees");

            // var filter = Builders<Employee>.Filter.And(Builders<Employee>.Filter.Where(x => x.Id == employeeId),
            //   Builders<Employee>.Filter.ElemMatch(x => x.Positions, x => x.StartDate == pos.StartDate && x.Position == pos.Position));

            var filter = Builders<Employee>.Filter.And(Builders<Employee>.Filter.Where(x => x.Id == employeeId),
                Builders<Employee>.Filter.ElemMatch(x => x.Positions, x => x.Id == pos.Id));

            var update = Builders<Employee>.Update.Set("Positions.$.ShopId", pos.ShopId);
            var result = collection.UpdateOne(filter, update);
            return result.IsAcknowledged && result.ModifiedCount >= 1;
        }

        /// <summary>
        /// Associates a new employement position with a already existing employee.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="employeeId"></param>
        /// <returns>True, if a successful update took place. False if update failed or no update was performed</returns>
        public bool AddPositionToEmployee(EmployementPosition position, ObjectId employeeId)
        {
            if (position.Id == null || position.Id.Equals(ObjectId.Empty))
            {
                position.Id = ObjectId.GenerateNewId();
            }
            var collection = db.GetCollection<Employee>("employees");
            var filter = Builders<Employee>.Filter.Eq("Id", employeeId);
            var update = Builders<Employee>.Update.Push("Positions", position);
            var result = collection.UpdateOne(filter, update);
            return result.IsAcknowledged && result.ModifiedCount >= 1;
        }

        public IEnumerable<EmployementPosition> GetPositionsOfEmployee(ObjectId employeeId)
        {
            var collection = db.GetCollection<Employee>("employees");
            var positions = collection.AsQueryable().Where(e => e.Id == employeeId).SelectMany(e => e.Positions);
            return positions;
        }

        /// <summary>
        /// Deletes an employement position from an already existing employee.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="employeeId"></param>
        /// <returns>True, if position was deleted. False if the operation failed or no update was performed</returns>
        public bool RemovePositionFromEmployee(EmployementPosition position, ObjectId employeeId)
        {
            var collection = db.GetCollection<Employee>("employees");

            var update = Builders<Employee>.Update.PullFilter(p => p.Positions,
                                                f => f.StartDate == position.StartDate && f.FullTimeProcent == position.FullTimeProcent
                                                && f.Position == position.Position && f.WorkLocationId == position.WorkLocationId);
            var result = collection
                .UpdateOne(p => p.Id == employeeId, update);

            return result.IsAcknowledged && result.ModifiedCount >= 1;
        }


        /// <summary>
        /// Associates a comment with an already existing employee in the database.
        /// </summary>
        /// <param name="comment"></param>
        /// <param name="employeeId"></param>
        /// <returns>True, if a successful update took place. False if update failed or no update was performed</returns>
        public bool AddCommentToEmployee(Comment comment, ObjectId employeeId)
        {
            var collection = db.GetCollection<Employee>("employees");
            var filter = Builders<Employee>.Filter.Eq("Id", employeeId);
            var update = Builders<Employee>.Update.Push("Comments", comment);
            var result = collection.UpdateOne(filter, update);
            return result.IsAcknowledged && result.ModifiedCount >= 1;
        }

        /// <summary>
        /// Checks if the provided employement position specifier is associated with a manager position 
        /// (location or corporate sales manager).
        /// </summary>
        public bool IsManager(int position)
        {
            var accessRules = GetAccessRules();

            var positions = accessRules.Where(a => a.UserRoles.Any(r => r.Equals(UserRole.CreateLocationReports))).Select(p => p.Position);
            return positions.Contains(position);
        }

        /// <summary>
        /// Checks if the provided employement position specifier is associated with a corporate sales manager position.
        /// </summary>
        public bool IsCorporateSalesManager(int position)
        {
            var accessRules = GetAccessRules();

            var positions = accessRules.Where(a => a.UserRoles.Any(r => r.Equals(UserRole.CreateGlobalReports))).Select(p => p.Position);
            return positions.Contains(position);
        }

        /// <summary>
        /// Checks if the provided employement position specifier is associated with a location manager position.
        /// </summary>
        public bool IsLocationManager(int position)
        {
            var accessRules = GetAccessRules();

            var positions = accessRules.Where(a => a.UserRoles.Any(r => r.Equals(UserRole.ReadWriteEmployee)) && !a.UserRoles.Any(r => r.Equals(UserRole.CreateGlobalReports))).Select(p => p.Position);
            return positions.Contains(position);
        }

        /// <summary>
        /// Gets the range of all products sold at a specific location, with localized names and prices.
        /// </summary>
        public ProductRange GetProductRangeForLocation(Location location)
        {
            var coll = db.GetCollection<ProductRange>("productRanges");
            var ranges = coll.AsQueryable();
            var range = coll.AsQueryable().Where(l => l.LocationId == location.Id).Single();
            return range as ProductRange;
        }

        /// <summary>
        /// Get all products sold by BeaverCoffee AB, without localization properties.
        /// </summary>
        public IEnumerable<Product> GetAllProducts()
        {
            var coll = db.GetCollection<Product>("products");

            var products = coll.Find(_ => true);
            return products.ToEnumerable();

        }

        /// <summary>
        /// Get those products that consist of other ingredients.
        /// </summary>
        public IEnumerable<Product> GetAllProductsWithIngredients()
        {
            var coll = db.GetCollection<Product>("products");
            var products = coll.AsQueryable().Where(p => p.Ingredients != null);
            return products.ToEnumerable();
        }

        public string EnglishNameOfProduct(ObjectId productId)
        {
            var collection = db.GetCollection<Product>("products");
            return collection.AsQueryable().Where(p => p.Id == productId).FirstOrDefault().EngName;
        }


        public bool AddCustomer(Customer customer)
        {
            var collection = db.GetCollection<Customer>("customer");

            try
            {
                collection.InsertOne(customer);
                return true;
            }
            catch (Exception e)
            {
                Error = e.Message;
                return false;
            }

        }

        /// <summary>
        /// Returns all customers who were registered at a specific BeaverCoffee location (e.g. US, Sweden)
        /// </summary>
        public IEnumerable<Customer> GetCuctomersForLocation(ObjectId locationId)
        {
            var collection = db.GetCollection<Customer>("customer");
            return collection.AsQueryable().Where(c => c.LocationId == locationId).ToEnumerable();
        }

        /// <summary>
        /// Saves a new employee in the database.
        /// </summary>
        /// <param name="employee">The Employeee to save</param>
        /// <returns>The id of the employee in the database if the operation was successful, an empty id otherwise</returns>
        public ObjectId AddEmployee(Employee employee)
        {
            {
                var collection = db.GetCollection<Employee>("employees");

                try
                {
                    collection.InsertOne(employee);
                    return employee.Id;
                }
                catch (Exception e)
                {
                    Error = e.Message;
                    return ObjectId.Empty;
                }
            }
        }

        /// <summary>
        /// Deletes the employeee from the database, by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>True, if deletion was successful. False if no deletion took place</returns>
        public bool RemoveEmployee(ObjectId id)
        {
            var collection = db.GetCollection<Employee>("employees");
            var filter = Builders<Employee>.Filter.Eq("Id", id);
            var result = collection.DeleteOne(filter);
            return result.IsAcknowledged && result.DeletedCount >= 1;
        }

        /// <summary>
        /// Gets the id of a Stock with the provided stock name.
        /// </summary>
        /// <param name="stockName">The name of the stock</param>
        /// <returns>The id of the stock if found in the database, an empty id otherwise</returns>
        public ObjectId GetStockIdForName(string stockName)
        {
            var collStocks = db.GetCollection<Stock>("stocks");
            var stock = collStocks.AsQueryable().Where(s => s.StockName == stockName).FirstOrDefault();
            return stock != null ? stock.Id : ObjectId.Empty;
        }

        /// <summary>
        /// Gets the name of the stock with a specific id in the database
        /// </summary>
        /// <param name="stockId">Stock id in database</param>
        /// <returns>The name of the Stock or null if stock was not found</returns>
        public string GetStockNameForId(ObjectId stockId)
        {
            var collStocks = db.GetCollection<Stock>("stocks");
            return collStocks.AsQueryable().Where(s => s.Id == stockId).FirstOrDefault().StockName;
        }

        /// <summary>
        /// Gets all the Stocks, storage units, at a specific location.
        /// </summary>
        /// <param name="locationId">The id of the country location where the stocks should be located</param>
        /// <returns></returns>
        public IEnumerable<Stock> GetStocksAtLocation(ObjectId locationId)
        {
            var collStocks = db.GetCollection<Stock>("stocks");
            return collStocks.AsQueryable().Where(s => s.LocationId == locationId).ToEnumerable();
        }

        /// <summary>
        /// Checks if there are enough units available in stock from a specific product
        /// </summary>
        /// <param name="stockId">The id of the stock to check for availability</param>
        /// <param name="productId">The product id</param>
        /// <param name="unitAvailable">The number of units that needs to be available</param>
        /// <returns>True if quantity is available, false otherwise</returns>
        public bool IsQuantityInStock(ObjectId stockId, ObjectId productId, double unitAvailable)
        {
            var collStockItems = db.GetCollection<StockItem>("stockitems");
            var stockItem = collStockItems.AsQueryable().Where(s => s.StockId == stockId && s.ProductId == productId).SingleOrDefault();
            if (stockItem != null)
                return stockItem.Amount >= unitAvailable;
            else
                return false;
        }

        /// <summary>
        /// Cheks the number units available in stock from a specific product.
        /// </summary>
        /// <param name="stockId">The id of the stock to check for availability</param>
        /// <param name="productId">The id of the product</param>
        /// <returns>Available quantity. Returns 0 for products not found in the given stock</returns>
        public double QuantityInStock(ObjectId stockId, ObjectId productId)
        {
            var collStock = db.GetCollection<StockItem>("stockitems");
            var stockItem = collStock.AsQueryable().Where(s => s.StockId == stockId && s.ProductId == productId).SingleOrDefault();
            if (stockItem != null)
                return stockItem.Amount;
            else
                return 0;
        }

        /// <summary>
        /// Performs a given change in the quantity (increase or decrease) of a list of stock items, if it is possible,
        /// and registers the quantity changes as stock transactions for the current day and specified stock unit.
        /// The stock items should all be associated with one specified Stock unit. If performing
        /// any of the quantity changes is not possible, due to e.g. not enough quantity in stock, the operation is aborted
        /// and an approporiate error message is set in the Error field of DbManager. 
        /// </summary>
        /// <param name="transactions">The quantity changes to perform as a list of Transaction objects</param>
        /// <param name="stockId">The id of the Stock where the quantity changes should take place.</param>
        /// <returns>True, if all the quantity changes were performed and saved, false otherwise</returns>
        public bool RegisterQuantityChangeInStock(IEnumerable<TransactionItem> transactions, ObjectId stockId)
        {
            var collTransactions = db.GetCollection<StockTransaction>("stocktransactions");

            DateTime today = DateTime.UtcNow;
            StockTransaction currentTransactionList = collTransactions.AsQueryable().Where(t => t.StockId == stockId).
                OrderByDescending(t => t.Day).FirstOrDefault();
            ObjectId transactionListId;


            if (currentTransactionList == null || today.Date != currentTransactionList.Day.Date)
            {
                var collStocks = db.GetCollection<Stock>("stocks");
                var locId = collStocks.AsQueryable().Where(s => s.Id == stockId).FirstOrDefault().Id;
                StockTransaction todaysTransactionList = new StockTransaction()
                {
                    StockId = stockId,
                    LocationId = locId != null ? locId : ObjectId.Empty,
                    Day = today.Date,
                    Transactions = new List<TransactionItem>(),
                };
                try
                {
                    collTransactions.InsertOne(todaysTransactionList);
                    transactionListId = todaysTransactionList.Id;
                }
                catch (Exception e)
                {
                    Error = "Could not create stock transaction list for the given stock";
                    return false;
                }
            }
            else
                transactionListId = currentTransactionList.Id;


            foreach (var item in transactions)
            {
                if (item.QuantityChange < 0 && QuantityInStock(stockId, item.ProductId) < item.QuantityChange)
                {
                    Error = "Not enough quantity in stock for item: " + item.ProductId;
                    return false;
                }
            }

            var collStockItems = db.GetCollection<StockItem>("stockitems");

            List<TransactionItem> transactionItemList = new List<TransactionItem>();
            foreach (var item in transactions)
            {
                item.StartingStockAmount = collStockItems.AsQueryable().Where(x => x.ProductId == item.ProductId && x.StockId == stockId).SingleOrDefault().Amount;
                double newAmount = item.StartingStockAmount + item.QuantityChange;
                var filter = Builders<StockItem>.Filter.Where(x => x.ProductId == item.ProductId && x.StockId == stockId);
                var update = Builders<StockItem>.Update.Set("Amount", newAmount);
                var updateObj = collStockItems.FindOneAndUpdate<StockItem>(filter, update);


                item.TimeStamp = DateTime.Now;
                transactionItemList.Add(item);
            }

            var filterTransaction = Builders<StockTransaction>.Filter.Eq("Id", transactionListId);
            var updateTransaction = Builders<StockTransaction>.Update.PushEach("Transactions", transactionItemList);
            var result = collTransactions.UpdateOne(filterTransaction, updateTransaction);

            return result.IsAcknowledged && result.ModifiedCount >=1;
        }

        /// <summary>
        /// Returns all the products and their quantity stored in a specific stock unit
        /// </summary>
        /// <param name="stockId">The stock unit to check</param>
        /// <returns>List of StockItems or null if there are not any at the specified Stock unit</returns>
        public IEnumerable<StockItem> GetStockItemsInStock(ObjectId stockId)
        {
            var collStock = db.GetCollection<StockItem>("stockitems");
            return collStock.AsQueryable().Where(s => s.StockId == stockId);
        }

        /// <summary>
        /// Registers the given new amount for the quantity of a stock item.
        /// </summary>
        /// <param name="stockItemId">The id of the stock item to update</param>
        /// <param name="newAmount">The new amount to set for the quantity of the stock item</param>
        /// <returns></returns>
        public bool UpdateStockItemAmount(ObjectId stockItemId, double newAmount)
        {
            var collStockItems = db.GetCollection<StockItem>("stockitems");
            var filter = Builders<StockItem>.Filter.Eq("Id", stockItemId);
            var update = Builders<StockItem>.Update.Set("Amount", newAmount);
            var result = collStockItems.UpdateOne(filter, update);
            return result.IsAcknowledged && result.ModifiedCount >= 1;
        }

        /// <summary>
        /// Gets all stock transactions registered for a given product at a specific BeaverCoffee location
        /// during the provided time interval.
        /// </summary>
        /// <param name="locationId">The id of the BeaverCoffee location</param>
        /// <param name="productId">The id of the product to get transactions for</param>
        /// <param name="start">The start date of the time interval</param>
        /// <param name="end">The end date of the time interval</param>
        /// <returns>Tuples with id of the stock as Item1 and a list of transactions for the product in that stock as Item2</returns>
        public IEnumerable<Tuple<ObjectId, IEnumerable<TransactionItem>>> GetStockTransactionsForProductAtLocationAndInterval(ObjectId locationId, ObjectId productId, DateTime start, DateTime end)
        {
            var stocks = GetStocksAtLocation(locationId);
            if (stocks != null)
            {
                var stockIds = stocks.Select(s => s.Id);
                var collTransactions = db.GetCollection<StockTransaction>("stocktransactions");

                IMongoQueryable<Tuple <ObjectId, IEnumerable<TransactionItem>>> result = collTransactions.AsQueryable().
                    Where(t => stockIds.Contains(t.StockId)
                    && t.Day >= start.Date && t.Day <= end.Date && t.Transactions.Any(i => i.ProductId == productId)).
                    Select(t => new Tuple<ObjectId, IEnumerable<TransactionItem>>(t.StockId, t.Transactions.Where(i => i.ProductId == productId)));

                return result.ToEnumerable();
             }
            return null;

        }

        /// <summary>
        /// Gets all stock transactions registered for a given product at any BeaverCoffee location
        /// during the provided time interval.
        /// </summary>
        /// <param name="productId">The id of the product to get transactions for</param>
        /// <param name="start">The start date of the time interval</param>
        /// <param name="end">The end date of the time interval</param>
        /// <returns>Tuples with id of the stock as Item1 and a list of transactions for the product in that stock as Item2</returns>
        public IEnumerable<Tuple<ObjectId, IEnumerable<TransactionItem>>> GetStockTransactionsForProductInInterval(ObjectId productId, DateTime start, DateTime end)
        {
            var collTransactions = db.GetCollection<StockTransaction>("stocktransactions");

            IMongoQueryable<Tuple<ObjectId, IEnumerable<TransactionItem>>> result = collTransactions.AsQueryable().
                Where(t => t.Day >= start.Date && t.Day <= end.Date && t.Transactions.Any(i => i.ProductId == productId)).
                Select(t => new Tuple<ObjectId, IEnumerable<TransactionItem>>(t.StockId, t.Transactions.Where(i => i.ProductId == productId)));
            return result.ToEnumerable();

        }

        /// <summary>
        /// Gets the most recent stock transaction registered for the given product at the provided
        /// BeaverCoffee location.
        /// </summary>
        public TransactionItem GetLatestStockTransactionForProductAtLocation(ObjectId locationId, ObjectId productId)
        {
            var stocks = GetStocksAtLocation(locationId);
            if (stocks != null)
            {
                var stockIds = stocks.Select(s => s.Id);
                var collTransactions = db.GetCollection<StockTransaction>("stocktransactions");

                return collTransactions.AsQueryable().Where(t => stockIds.Contains(t.StockId))
                    .SelectMany(t => t.Transactions).Where(t => t.ProductId == productId).OrderByDescending(t => t.TimeStamp).FirstOrDefault();
            }
            return null;
        }

        /// <summary>
        /// Gets the most recent stock transaction registered for the given product at any
        /// BeaverCoffee location.
        /// </summary>
        public TransactionItem GetLatestStockTransactionForProduct(ObjectId productId)
        {
            var collTransactions = db.GetCollection<StockTransaction>("stocktransactions");

            return collTransactions.AsQueryable()
                .SelectMany(t => t.Transactions).Where(t => t.ProductId == productId).OrderByDescending(t => t.TimeStamp).FirstOrDefault();

        }


        public bool SaveOrder(Order order)
        {
            var collection = db.GetCollection<Order>("orders");
            try
            {
                collection.InsertOne(order);
                return true;
            }
            catch (Exception ex)
            {
                Error = ex.Message;
                return false;
            }
        }

        public IEnumerable<Order> GetAllOrders()
        {
            var collection = db.GetCollection<Order>("orders");
            var orders = collection.Find(_ => true);
            return orders.ToEnumerable();
        }

        /// <summary>
        /// Returns all coffee shop orders that have been registered at a given location.
        /// </summary>
        public IEnumerable<Order> GetOrdersForLocation(ObjectId locationId)
        {
            var collection = db.GetCollection<Order>("orders");
            return collection.AsQueryable().Where(o => o.LocationId == locationId);
        }

        /// <summary>
        /// Gets all orders registered in the given time interval at the specific location.
        /// </summary>
        public IEnumerable<Order> GetOrdersInDateIntervalForLocation(ObjectId locationId, DateTime start, DateTime end)
        {
            var collection = db.GetCollection<Order>("orders");
            return collection.AsQueryable().Where(o => o.LocationId == locationId && o.OrderDate >= start && o.OrderDate <= end);
        }

        /// <summary>
        /// Gets all orders registered in the given time interval at any location.
        /// </summary>
        public IEnumerable<Order> GetAllOrdersInDateInterval(DateTime start, DateTime end)
        {
            var collection = db.GetCollection<Order>("orders");
            return collection.AsQueryable().Where(o => o.OrderDate >= start && o.OrderDate <= end);
        }

        /// <summary>
        /// Gets all orders served by the specified employee at the given BeaverCoffee location.
        /// </summary>
        public IEnumerable<Order> GetOrdersForLocationPerEmployee(ObjectId locationId, ObjectId employeeId)
        {
            var collection = db.GetCollection<Order>("orders");
            return collection.AsQueryable().Where(o => o.LocationId == locationId && o.RegisteringEmployeeId == employeeId);
        }

        /// <summary>
        /// Gets all orders served by the specified employee across all BeaverCoffee locations.
        /// </summary>
        public IEnumerable<Order> GetOrdersPerEmployee(ObjectId employeeId)
        {
            var collection = db.GetCollection<Order>("orders");
            return collection.AsQueryable().Where(o => o.RegisteringEmployeeId == employeeId);
        }

        /// <summary>
        /// Gets all orders served by the specified employee at the given BeaverCoffee location
        /// in the specified time interval.
        /// </summary>
        public IEnumerable<Order> GetOrdersInDateIntervalForLocationPerEmployee(ObjectId locationId, ObjectId employeeId, DateTime start, DateTime end)
        {
            var collection = db.GetCollection<Order>("orders");
            return collection.AsQueryable().Where(o => o.LocationId == locationId && o.RegisteringEmployeeId == employeeId && o.OrderDate >= start && o.OrderDate <= end);
        }

        /// <summary>
        /// Returns the currency code associated with the given BeaverCoffee location
        /// </summary>
        public Currency CurrencyForLocation(ObjectId locatonId)
        {
            var collection = db.GetCollection<Location>("locations");
            return collection.AsQueryable().Where(p => p.Id == locatonId).SingleOrDefault().Currency;
        }

        public void DeleteAllOrders()
        {
            var collOrders = db.GetCollection<Order>("orders");
            collOrders.DeleteMany(FilterDefinition<Order>.Empty);
        }

        /// <summary>
        /// Deletes a specified Order from the database.
        /// </summary>
        /// <param name="orderId">The id of the order to delete</param>
        /// <returns>True if order was deleted, false if no deletion took place</returns>
        public bool DeleteOrder(ObjectId orderId)
        {
            var collOrders = db.GetCollection<Order>("orders");
            var filter = Builders<Order>.Filter.Eq("Id", orderId);
            var result = collOrders.DeleteOne(filter);
            return result.IsAcknowledged && result.DeletedCount >= 1;
        }

        /// <summary>
        /// Gets all orders served in the specified time interval by any employee at the specified location.
        /// </summary>
        public IEnumerable<Order> GetOrdersInInterval(ObjectId locationId, DateTime start, DateTime end)
        {
            var collOrders = db.GetCollection<Order>("orders");
            return collOrders.AsQueryable().Where(o => o.LocationId == locationId && o.OrderDate >= start && o.OrderDate <= end);
        }

        /// <summary>
        /// Gets all orders served in the specified time interval by any employee across all locations.
        /// </summary>
        public IEnumerable<Order> GetAllOrdersInInterval(DateTime start, DateTime end)
        {
            var collOrders = db.GetCollection<Order>("orders");
            return collOrders.AsQueryable().Where(o => o.OrderDate >= start && o.OrderDate <= end);
        }

        /// <summary>
        /// Gets all AccessRules defined in the database, underlying access control in the application.
        /// </summary>
        public IEnumerable<AccessRule> GetAccessRules()
        {
            var collection = db.GetCollection<AccessRule>("accessRules");
            var orders = collection.Find(_ => true);
            return orders.ToEnumerable();
        }

    }
}

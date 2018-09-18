using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver.Linq;
using MongoDB.Driver;
using MongoDB.Bson;

namespace BeaverCoffeeDemo.Data
{
    public class SeedDb
    {
        public string Error { get; set; }
        private IMongoDatabase db;

        public SeedDb()
        {
            IMongoClient client = new MongoClient();
            db = client.GetDatabase("coffeeshop");
        }

        public bool Seed()
        {
            var collOrders = db.GetCollection<Order>("orders");
            var collTransactions = db.GetCollection<StockTransaction>("stocktransactions");
            var collStocks = db.GetCollection<Stock>("stocks");
            var collStockItems = db.GetCollection<StockItem>("stockitems");
            var collProducts = db.GetCollection<Product>("products");
            var collLocations = db.GetCollection<Location>("locations");
            var collProductRange = db.GetCollection<ProductRange>("productRanges");
            var collCustomer = db.GetCollection<Customer>("customer");
            var collEmployee = db.GetCollection<Employee>("employees");
            var collRoles = db.GetCollection<AccessRule>("accessRules");


            collOrders.DeleteMany(FilterDefinition<Order>.Empty);
            collTransactions.DeleteMany(FilterDefinition<StockTransaction>.Empty);
            collStockItems.DeleteMany(FilterDefinition<StockItem>.Empty);
            collStocks.DeleteMany(FilterDefinition<Stock>.Empty);
            collProductRange.DeleteMany(FilterDefinition<ProductRange>.Empty);
            collProducts.DeleteMany(FilterDefinition<Product>.Empty);
            collCustomer.DeleteMany(FilterDefinition<Customer>.Empty);
            collRoles.DeleteMany(FilterDefinition<AccessRule>.Empty);
            collEmployee.DeleteMany(FilterDefinition<Employee>.Empty);
            collLocations.DeleteMany(FilterDefinition<Location>.Empty);


            try
            {
                AddLocations();
                AddProducts();
                AddUSProductRange();
                AddSweProductRange();
                AddEmployees();
                AddAccessRules();
                AddStockItems();
                AddCustomers();
                return true;
            }
            catch(Exception e)
            {
                return false;
            }

        }


        public void AddProducts()
        {
            AddBaggedCoffees();
            AddBaseIngredients();
            AddMilkTypeIngredients();
            AddFlavourings();
            AddExtraIngredients();
            AddBeverages();
        }

        public void AddBaggedCoffees()
        {
            Product b1 = new Product()
            {
                EngName = "espresso roast",
                Category = ProductCategory.bagged_coffee,
            };

            Product b2 = new Product()
            {
                EngName = "whole bean french roast",
                Category = ProductCategory.bagged_coffee,
            };

            Product b3 = new Product()
            {
                EngName = "whole bean light roast",
                Category = ProductCategory.bagged_coffee,
            };


            List<Product> bagged = new List<Product>();
            bagged.Add(b1);
            bagged.Add(b2);
            bagged.Add(b3);

            var collection = db.GetCollection<Product>("products");
            collection.InsertMany(bagged);

        }

        public void AddBaseIngredients()
        {
            Product b1 = new Product()
            {
                EngName = "espresso",
                Category = ProductCategory.base_coffee,
            };

            Product b2 = new Product()
            {
                EngName = "brewed coffee",
                Category = ProductCategory.base_coffee,
            };

            Product b3 = new Product()
            {
                EngName = "cocoa mix",
                Category = ProductCategory.base_noncoffee,
            };

            List<Product> bases = new List<Product>();
            bases.Add(b1);
            bases.Add(b2);
            bases.Add(b3);


            try
            {
                var collection = db.GetCollection<Product>("products");
                collection.InsertMany(bases);
            }
            catch (Exception ex)
            {
                Error = "Error adding base ingredients to the database: " + ex.Message;
                throw;
            }
        }

        public void AddFlavourings()
        {
            Product f1 = new Product()
            {
                EngName = "vanilla",
                Category = ProductCategory.flavouring,
            };

            Product f2 = new Product()
            {
                EngName = "caramel",
                Category = ProductCategory.flavouring,
            };

            Product f3 = new Product()
            {
                EngName = "irish cream",
                Category = ProductCategory.flavouring,
            };

            List<Product> flavourings = new List<Product>();
            flavourings.Add(f1);
            flavourings.Add(f2);
            flavourings.Add(f3);


            try
            {
                var collection = db.GetCollection<Product>("products");
                collection.InsertMany(flavourings);

            }
            catch (Exception ex)
            {
                Error = "Error adding flavourings to the database: " + ex.Message;
                throw;
            }
        }

        public void AddExtraIngredients()
        {
            Product e1 = new Product()
            {
                EngName = "whipped cream",
                Category = ProductCategory.extra_ingredient,
            };


            try
            {
                var collection = db.GetCollection<Product>("products");
                collection.InsertOne(e1);
            }
            catch (Exception ex)
            {
                Error = "Error adding extra ingredient(s) to the database: " + ex.Message;
                throw;
            }
        }

        public void AddMilkTypeIngredients()
        {


            Product m1 = new Product()
            {
                EngName = "whole milk",
                Category = ProductCategory.milk_type,
            };


            Product m2 = new Product()
            {
                EngName = "2percent milk",
                Category = ProductCategory.milk_type,
            };


            Product m3 = new Product()
            {
                EngName = "soy milk",
                Category = ProductCategory.milk_type,
            };

            Product m4 = new Product()
            {
                EngName = "skim milk",
                Category = ProductCategory.milk_type,
            };

            List<Product> milkTypes = new List<Product>();
            milkTypes.Add(m1);
            milkTypes.Add(m3);
            milkTypes.Add(m4);
            milkTypes.Add(m2);

            try
            {
                var collection = db.GetCollection<Product>("products");
                collection.InsertMany(milkTypes);
            }
            catch (Exception ex)
            {
                Error = "Error adding milk type ingredients to the database: " + ex.Message;
                throw;
            }

        }

        public void AddBeverages()
        {

            var collection = db.GetCollection<Product>("products");

            Product p1 = new Product()
            {
                EngName = "Hot chocolate",
                Category = ProductCategory.beverage_noncoffee,
                Ingredients = new Ingredients()
            };

            List<BaseIngredient> p1BaseIngredients = new List<BaseIngredient>();
            p1BaseIngredients.Add(new BaseIngredient()
            {
                Id = collection.AsQueryable<Product>().Where(e => e.EngName == "cocoa mix").Single().Id,
                StockUnit = 1
            });

            p1.Ingredients.BaseIngredients = p1BaseIngredients;

            MilkTypeIngredients p1MilkIngredients = new MilkTypeIngredients()
            {
                StockUnit = 2,
            };
            List<ObjectId> milkOptions = new List<ObjectId>();
            milkOptions.Add(collection.AsQueryable<Product>().Where(e => e.EngName == "whole milk").Single().Id);
            milkOptions.Add(collection.AsQueryable<Product>().Where(e => e.EngName == "2percent milk").Single().Id);
            milkOptions.Add(collection.AsQueryable<Product>().Where(e => e.EngName == "soy milk").Single().Id);
            milkOptions.Add(collection.AsQueryable<Product>().Where(e => e.EngName == "skim milk").Single().Id);
            p1MilkIngredients.Options = milkOptions;

            p1.Ingredients.MilkTypeIngredients = p1MilkIngredients;

            List<ObjectId> extraIngredients = new List<ObjectId>();
            extraIngredients.Add(collection.AsQueryable<Product>().Where(e => e.EngName == "whipped cream").Single().Id);

            p1.Ingredients.ExtraIngredients = extraIngredients;


            Product p2 = new Product()
            {
                EngName = "Latte",
                Category = ProductCategory.beverage_coffee,
                Ingredients = new Ingredients()
            };

            List<BaseIngredient> p2BaseIngredients = new List<BaseIngredient>();
            p2BaseIngredients.Add(new BaseIngredient()
            {
                Id = collection.AsQueryable<Product>().Where(e => e.EngName == "espresso").Single().Id,
                StockUnit = 1
            });

            p2.Ingredients.BaseIngredients = p2BaseIngredients;

            MilkTypeIngredients p2MilkIngredients = new MilkTypeIngredients()
            {
                StockUnit = 2,
            };

            p2MilkIngredients.Options = milkOptions;

            p2.Ingredients.MilkTypeIngredients = p2MilkIngredients;



            Product p3 = new Product()
            {
                EngName = "Capuccino",
                Category = ProductCategory.beverage_coffee,
                Ingredients = new Ingredients()
            };

            List<BaseIngredient> p3BaseIngredients = new List<BaseIngredient>();
            p3BaseIngredients.Add(new BaseIngredient()
            {
                Id = collection.AsQueryable<Product>().Where(e => e.EngName == "espresso").Single().Id,
                StockUnit = 1
            });

            p3.Ingredients.BaseIngredients = p3BaseIngredients;

            MilkTypeIngredients p3MilkIngredients = new MilkTypeIngredients()
            {
                StockUnit = 1,
            };

            p3MilkIngredients.Options = milkOptions;

            p3.Ingredients.MilkTypeIngredients = p3MilkIngredients;

            List<Product> products = new List<Product>();
            products.Add(p1);
            products.Add(p2);
            products.Add(p3);

            try
            {
                collection.InsertMany(products);

            }
            catch (Exception ex)
            {
                Error = "Error adding beverages to the database: " + ex.Message;
                throw;
            }
        }


        public void AddLocations()
        {
            Shop shopSw1 = new Shop() {
                Address = new Address() {
                    City = "Malmoe",
                    Country = "Sweden",
                    Street = "Storgatan 2",
                    Zip = "12345"
                },
                Id = ObjectId.GenerateNewId(),
            };

            //Shop shopSw2 = new Shop()
            //{
            //    Address = new Address()
            //    {
            //        City = "Stockholm",
            //        Country = "Sweden",
            //        Street = "Testgatan 34",
            //        Zip = "23456"
            //    },
            //    Id = ObjectId.GenerateNewId(),
            //};

            Shop shopUS1 = new Shop() {
                Address = new Address() {
                    Country = "US",
                    City = "SF",
                    Street = "Main square 4",
                    Zip = "243768"
                },
                Id = ObjectId.GenerateNewId(),
            };

            Shop shopUS2 = new Shop()
            {
                Address = new Address()
                {
                    Country = "US",
                    City = "SF",
                    Street = "Example street 23",
                    Zip = "876654"
                },
                Id = ObjectId.GenerateNewId(),
            };

            List<Shop> sweShops = new List<Shop>();
            sweShops.Add(shopSw1);
            //sweShops.Add(shopSw2);

            List<Shop> usShops = new List<Shop>();
            usShops.Add(shopUS1);
            usShops.Add(shopUS2);

            List<Location> locations = new List<Location>();

            locations.Add(new Location()
            {
                Name = "Sweden",
                Currency = Currency.SEK,
                LanguageCode = LanguageCode.Swe,
                PersonalIdFormat = @"(?<!\d)\d{10}(?!\d)",
                Shops = sweShops
            });

            locations.Add(new Location()
            {
                Name = "US",
                Currency = Currency.USD,
                LanguageCode = LanguageCode.Eng,
                PersonalIdFormat = "^[A-Za-z]{6}[0-9]{2}$", // Made up format
                Shops = usShops

            });

            try
            {
                var collection = db.GetCollection<Location>("locations");
                collection.InsertMany(locations);
            }
            catch (Exception ex)
            {
                Error = "Error adding coffee shop locations to the database: " + ex.Message;
                throw;
            }

        }

        public void AddEmployees()
        {

            var collLocations = db.GetCollection<Location>("locations");
            var locationUs = collLocations.AsQueryable().Where(l => l.Currency == Currency.USD).Single();
            var locationSwe = collLocations.AsQueryable().Where(l => l.Currency == Currency.SEK).Single();

            var collEmployee = db.GetCollection<Employee>("employees");

            EmployementPosition shopWorkerUs1pos1 = new EmployementPosition()
            {
                Id = ObjectId.GenerateNewId(),
                StartDate = new DateTime(2016, 4, 29, 00, 00, 00),
                EndDate = new DateTime(2017, 5, 30, 00, 00, 00),
                Position = 1,
                FullTimeProcent = 100,
                WorkLocationId = locationUs.Id,
                ShopId = locationUs.Shops.Where(s => s.Address.Street == "Main square 4" && s.Address.City == "SF").Single().Id
            };

            EmployementPosition shopWorkerUs1pos2 = new EmployementPosition()
            {
                Id = ObjectId.GenerateNewId(),
                StartDate = new DateTime(2017, 6, 01, 00, 00, 00),
                Position = 1,
                FullTimeProcent = 100,
                WorkLocationId = locationUs.Id,
                ShopId = locationUs.Shops.Where(s => s.Address.Street == "Main square 4" && s.Address.City == "SF").Single().Id,
            };

            List<EmployementPosition> positionsUS1 = new List<EmployementPosition>();
            positionsUS1.Add(shopWorkerUs1pos1);
            positionsUS1.Add(shopWorkerUs1pos2);


            Employee empUS1 = new Employee()
            {
                Name = "Tom Smith",
                PersonalId = "TSSFDR23",
                HireDate = new DateTime(2016, 4, 29, 00, 00, 00),
                Address = new Address()
                {
                    City = "San Francisco",
                    Street = "Test avenue 34",
                    Country = "US",
                    Zip = "23445"
                },
                Positions = positionsUS1,
                Comments = new List<Comment>(),
            };



            EmployementPosition shopWorkerUs2pos1 = new EmployementPosition()
            {
                Id = ObjectId.GenerateNewId(),
                StartDate = new DateTime(2017, 4, 29, 00, 00, 00),
                Position = 1,
                FullTimeProcent = 50,
                WorkLocationId = locationUs.Id,
                ShopId = locationUs.Shops.Where(s => s.Address.Street == "Example street 23" && s.Address.City == "SF").Single().Id,
            };

            List<EmployementPosition> positionsUS2 = new List<EmployementPosition>();
            positionsUS2.Add(shopWorkerUs2pos1);


            Employee empUS2 = new Employee()
            {
                Name = "Anna L",
                PersonalId = "ALDTRE75",
                HireDate = new DateTime(2017, 4, 29, 00, 00, 00),
                Address = new Address()
                {
                    City = "Oakland",
                    Street = "Test stret 1",
                    Country = "US",
                    Zip = "45634"
                },
                Positions = positionsUS2,
                Comments = new List<Comment>(),
            };

            List<Employee> employees = new List<Employee>();
            employees.Add(empUS1);
            employees.Add(empUS2);

            Shop sweShop = locationSwe.Shops.Where(s => s.Address.Street == "Storgatan 2").Single();

            EmployementPosition shopWorkerSwe1pos1 = new EmployementPosition()
            {
                Id = ObjectId.GenerateNewId(),
                StartDate = new DateTime(2015, 4, 29, 00, 00, 00),
                EndDate = new DateTime(2016, 5, 30, 00, 00, 00),
                Position = 1,
                FullTimeProcent = 100,
                WorkLocationId = locationSwe.Id,
                ShopId = sweShop.Id,
            };

            EmployementPosition shopWorkerSwe1pos2 = new EmployementPosition()
            {
                Id = ObjectId.GenerateNewId(),
                StartDate = new DateTime(2016, 6, 01, 00, 00, 00),
                Position = 1,
                FullTimeProcent = 100,
                WorkLocationId = locationSwe.Id,
                ShopId = sweShop.Id,
            };

            List<EmployementPosition> positionSwe1 = new List<EmployementPosition>();
            positionSwe1.Add(shopWorkerSwe1pos1);
            positionSwe1.Add(shopWorkerSwe1pos2);


            Employee empSwe1 = new Employee()
            {
                Name = "Lars Andersson",
                PersonalId = "8902135300",
                HireDate = new DateTime(2015, 4, 29, 00, 00, 00),
                Address = new Address()
                {
                    City = "Malmoe",
                    Street = "Test gatan 5",
                    Country = "Sweden",
                    Zip = "25463"
                },
                Positions = positionSwe1,
                Comments = new List<Comment>(),
            };

            employees.Add(empSwe1);


            List<EmployementPosition> positionSwe2 = new List<EmployementPosition>();
            positionSwe2.Add(shopWorkerSwe1pos2);

            Employee empSwe2 = new Employee()
            {
                Name = "Elza M",
                PersonalId = "9202135300",
                HireDate = new DateTime(2016, 6, 01, 00, 00, 00),
                Address = new Address()
                {
                    City = "Malmoe",
                    Street = "Test torget 15",
                    Country = "Sweden",
                    Zip = "25463"
                },
                Positions = positionSwe2,
                Comments = new List<Comment>(),
            };

            employees.Add(empSwe2);

            try
            {
                collEmployee.InsertMany(employees);
            }
            catch (Exception ex)
            {
                Error = "Error adding coffee shop employees to the database: " + ex.Message;
                throw;
            }

            var bossOverEmployeesUS = collEmployee.AsQueryable().Where(e => e.Positions.Any(p => p.WorkLocationId == locationUs.Id && p.EndDate < new DateTime(100, 01, 01, 00, 00, 00)));

            List<ObjectId> bossOverList = new List<ObjectId>();
            foreach (var emp in bossOverEmployeesUS)
            {
                bossOverList.Add(emp.Id);
            }

            EmployementPosition bosPossUs1 = new EmployementPosition()
            {
                Id = ObjectId.GenerateNewId(),
                StartDate = new DateTime(2017, 1, 01, 00, 00, 00),
                Position = 2,
                FullTimeProcent = 100,
                WorkLocationId = locationUs.Id,
                BossOver = bossOverList,
            };


            List<EmployementPosition> bossPositionsUS1 = new List<EmployementPosition>();
            bossPositionsUS1.Add(bosPossUs1);

            Employee bossUS1 = new Employee()
            {
                Name = "Kate M",
                PersonalId = "KMDRET28",
                HireDate = new DateTime(2017, 1, 01, 00, 00, 00),
                Address = new Address()
                {
                    City = "San Francisco",
                    Street = "Test street 234",
                    Country = "US",
                    Zip = "98756"
                },
                Positions = bossPositionsUS1,
                Comments = new List<Comment>(),
            };

            List<Employee> bosses = new List<Employee>();
            bosses.Add(bossUS1);


            var bossOverEmployeesSwe = collEmployee.AsQueryable().Where(e => e.Positions.Any(p => p.WorkLocationId == locationSwe.Id && p.EndDate < new DateTime(100, 01, 01, 00, 00, 00)));

            List<ObjectId> bossOverListSwe = new List<ObjectId>();
            foreach (var emp in bossOverEmployeesSwe)
            {
                bossOverListSwe.Add(emp.Id);
            }

            EmployementPosition bosPossSwe1 = new EmployementPosition()
            {
                Id = ObjectId.GenerateNewId(),
                StartDate = new DateTime(2017, 1, 01, 00, 00, 00),
                Position = 2,
                FullTimeProcent = 100,
                WorkLocationId = locationSwe.Id,
                BossOver = bossOverListSwe,
            };


            List<EmployementPosition> bossPositionsSwe1 = new List<EmployementPosition>();
            bossPositionsSwe1.Add(bosPossSwe1);

            Employee bossSwe1 = new Employee()
            {
                Name = "Jon L",
                PersonalId = "7812065321",
                HireDate = new DateTime(2017, 1, 01, 00, 00, 00),
                Address = new Address()
                {
                    City = "Lund",
                    Street = "Test torget 1",
                    Country = "Sweden",
                    Zip = "25231"
                },
                Positions = bossPositionsSwe1,
                Comments = new List<Comment>(),
            };

            bosses.Add(bossSwe1);


            try
            {
                collEmployee.InsertMany(bosses);
            }
            catch (Exception ex)
            {
                Error = "Error adding boss employees to the database: " + ex.Message;
                throw;
            }

            List<ObjectId> bossOverbosses = new List<ObjectId>();
            bossOverbosses.Add(bossSwe1.Id);
            bossOverbosses.Add(bossUS1.Id);

            EmployementPosition managerPos = new EmployementPosition()
            {
                Id = ObjectId.GenerateNewId(),
                StartDate = new DateTime(2017, 1, 01, 00, 00, 00),
                Position = 3,
                FullTimeProcent = 100,
                WorkLocationId = locationSwe.Id,
                BossOver = bossOverbosses,
            };


            List<EmployementPosition> managerPositions = new List<EmployementPosition>();
            managerPositions.Add(managerPos);

            Employee manager = new Employee()
            {
                Name = "Lars G",
                PersonalId = "7208065334",
                HireDate = new DateTime(2017, 1, 01, 00, 00, 00),
                Address = new Address()
                {
                    City = "Lund",
                    Street = "Test gatan 34",
                    Country = "Sweden",
                    Zip = "25431"
                },
                Positions = managerPositions,
                Comments = new List<Comment>(),
            };

            try
            {
                collEmployee.InsertOne(manager);
            }
            catch (Exception ex)
            {
                Error = "Error adding manager employee to the database: " + ex.Message;
                throw;
            }

            Comment commentUS = new Comment()
            {
                EmployerId = bossUS1.Id,
                EmployerName = bossUS1.Name,
                Date = DateTime.Now,
                Text = "This is a comment about an employee written by an employer",
            };

            Comment commentSwe = new Comment()
            {
                EmployerId = bossSwe1.Id,
                EmployerName = bossSwe1.Name,
                Date = DateTime.Now,
                Text = "Det här är en kommentar...",
            };


            try
            {

                var filter = Builders<Employee>.Filter.Eq("Id", empUS1.Id);
                var update = Builders<Employee>.Update.Push("Comments", commentUS);
                var result = collEmployee.UpdateOne(filter, update);

                var filter2 = Builders<Employee>.Filter.Eq("Id", empSwe1.Id);
                var update2 = Builders<Employee>.Update.Push("Comments", commentSwe);
                var result2 = collEmployee.UpdateOne(filter2, update2);
            }
            catch (Exception ex)
            {
                Error = ex.Message;
                throw;
            }

        }

        public void AddAccessRules()
        {
            var collRoles = db.GetCollection<AccessRule>("accessRules");

            List<UserRole> userRoles1 = new List<UserRole>();
            userRoles1.Add(UserRole.ProcessOrder);
            userRoles1.Add(UserRole.RegisterCustomer);

            AccessRule rule1 = new AccessRule()
            {
                Position = 1,
                UserRoles = userRoles1,
            };

            List<UserRole> userRoles2 = new List<UserRole>();
            userRoles2.AddRange(userRoles1);
            userRoles2.Add(UserRole.CreateLocationReports);
            userRoles2.Add(UserRole.ReadWriteCustomer);
            userRoles2.Add(UserRole.ReadWriteEmployee);
            userRoles2.Add(UserRole.ReadWriteStock);

            AccessRule rule2 = new AccessRule()
            {
                Position = 2,
                UserRoles = userRoles2,
            };

            List<UserRole> userRoles3 = new List<UserRole>();
            userRoles3.AddRange(userRoles2);
            userRoles3.Add(UserRole.CreateGlobalReports);


            AccessRule rule3 = new AccessRule()
            {
                Position = 3,
                UserRoles = userRoles3,
            };

            List<AccessRule> ruleList = new List<AccessRule>();
            ruleList.Add(rule1);
            ruleList.Add(rule2);
            ruleList.Add(rule3);

            try
            {
                collRoles.InsertMany(ruleList);
            }
            catch (Exception ex)
            {
                Error = "Error adding user roles to the database: " + ex.Message;
                throw;
            }

        }


        public void AddSweProductRange()
        {

            var products = db.GetCollection<Product>("products");

            var locationCollection = db.GetCollection<Location>("locations");


            var location = locationCollection.AsQueryable().Where(e => e.Name == "Sweden").Single();

            ProductRange range = new ProductRange()
            {
                LocationId = location.Id,
            };

            ProductRangeItem p1 = new ProductRangeItem()
            {
                Id = products.AsQueryable<Product>().Where(e => e.EngName == "Latte").Single().Id,
                Name = "Caffe Latte",
                Price = 30,
                Category = ProductCategory.beverage_coffee
            };
            ProductRangeItem p2 = new ProductRangeItem()
            {
                Id = products.AsQueryable<Product>().Where(e => e.EngName == "Capuccino").Single().Id,
                Name = "Capuccino",
                Price = 25,
                Category = ProductCategory.beverage_coffee
            };
            ProductRangeItem p3 = new ProductRangeItem()
            {
                Id = products.AsQueryable<Product>().Where(e => e.EngName == "Hot chocolate").Single().Id,
                Name = "Varm Choklad",
                Price = 25,
                Category = ProductCategory.beverage_noncoffee
            };


            ProductRangeItem p4 = new ProductRangeItem()
            {
                Id = products.AsQueryable<Product>().Where(e => e.EngName == "skim milk").Single().Id,
                Name = "lättmjölk",
                Category = ProductCategory.milk_type
            };
            ProductRangeItem p5 = new ProductRangeItem()
            {
                Id = products.AsQueryable<Product>().Where(e => e.EngName == "soy milk").Single().Id,
                Name = "soja mjölk",
                Category = ProductCategory.milk_type
            };
            ProductRangeItem p6 = new ProductRangeItem()
            {
                Id = products.AsQueryable<Product>().Where(e => e.EngName == "whole milk").Single().Id,
                Name = "mjölk",
                Category = ProductCategory.milk_type
            };
            ProductRangeItem p7 = new ProductRangeItem()
            {
                Id = products.AsQueryable<Product>().Where(e => e.EngName == "2percent milk").Single().Id,
                Name = "mellanmjölk",
                Category = ProductCategory.milk_type
            };

            ProductRangeItem p8 = new ProductRangeItem()
            {
                Id = products.AsQueryable<Product>().Where(e => e.EngName == "whole bean french roast").Single().Id,
                Name = "whole bean french roast",
                Price = 35,
                Category = ProductCategory.bagged_coffee
            };

            ProductRangeItem p9 = new ProductRangeItem()
            {
                Id = products.AsQueryable<Product>().Where(e => e.EngName == "whole bean light roast").Single().Id,
                Name = "whole bean light roast",
                Price = 35,
                Category = ProductCategory.bagged_coffee
            };

            ProductRangeItem p10 = new ProductRangeItem()
            {
                Id = products.AsQueryable<Product>().Where(e => e.EngName == "espresso roast").Single().Id,
                Name = "espresso roast",
                Price = 35,
                Category = ProductCategory.bagged_coffee
            };

            ProductRangeItem p11 = new ProductRangeItem()
            {
                Id = products.AsQueryable<Product>().Where(e => e.EngName == "vanilla").Single().Id,
                Name = "vanilj",
                Price = 5,
                Category = ProductCategory.flavouring
            };

            ProductRangeItem p12 = new ProductRangeItem()
            {
                Id = products.AsQueryable<Product>().Where(e => e.EngName == "irish cream").Single().Id,
                Name = "irländsk kräm",
                Price = 5,
                Category = ProductCategory.flavouring
            };

            ProductRangeItem p13 = new ProductRangeItem()
            {
                Id = products.AsQueryable<Product>().Where(e => e.EngName == "caramel").Single().Id,
                Name = "karamell",
                Price = 5,
                Category = ProductCategory.flavouring
            };

            ProductRangeItem p14 = new ProductRangeItem()
            {
                Id = products.AsQueryable<Product>().Where(e => e.EngName == "whipped cream").Single().Id,
                Name = "grädde",
                Price = 4,
                Category = ProductCategory.extra_ingredient
            };

            ProductRangeItem p15 = new ProductRangeItem()
            {
                Id = products.AsQueryable<Product>().Where(e => e.EngName == "espresso").Single().Id,
                Name = "espresso",
                Price = 20,
                Category = ProductCategory.base_coffee
            };

            ProductRangeItem p16 = new ProductRangeItem()
            {
                Id = products.AsQueryable<Product>().Where(e => e.EngName == "brewed coffee").Single().Id,
                Name = "bryggkaffe",
                Price = 20,
                Category = ProductCategory.base_coffee
            };

            ProductRangeItem p17 = new ProductRangeItem()
            {
                Id = products.AsQueryable<Product>().Where(e => e.EngName == "cocoa mix").Single().Id,
                Name = "kakao mix",
                Category = ProductCategory.base_coffee
            };

            List<ProductRangeItem> lst = new List<ProductRangeItem>();
            lst.Add(p1);
            lst.Add(p2);
            lst.Add(p3);
            lst.Add(p4);
            lst.Add(p5);
            lst.Add(p6);
            lst.Add(p7);
            lst.Add(p8);
            lst.Add(p9);
            lst.Add(p10);
            lst.Add(p11);
            lst.Add(p12);
            lst.Add(p13);
            lst.Add(p14);
            lst.Add(p15);
            lst.Add(p16);
            lst.Add(p17);

            range.Products = lst;

            try
            {
                var coll = db.GetCollection<ProductRange>("productRanges");
                coll.InsertOne(range);
            }
            catch (Exception ex)
            {
                Error = "Error adding the Swedish product range to the database: " + ex.Message;
                throw;
            }


        }

        public void AddUSProductRange()
        {

            var products = db.GetCollection<Product>("products");

            var locationCollection = db.GetCollection<Location>("locations");


            var location = locationCollection.AsQueryable().Where(e => e.Name == "US").Single();

            ProductRange range = new ProductRange()
            {
                LocationId = location.Id,
            };

            ProductRangeItem p1 = new ProductRangeItem()
            {
                Id = products.AsQueryable<Product>().Where(e => e.EngName == "Latte").Single().Id,
                Name = "Latte",
                Price = 3,
                Category = ProductCategory.beverage_coffee
            };
            ProductRangeItem p2 = new ProductRangeItem()
            {
                Id = products.AsQueryable<Product>().Where(e => e.EngName == "Capuccino").Single().Id,
                Name = "Capuccino",
                Price = 2.8M,
                Category = ProductCategory.beverage_coffee
            };
            ProductRangeItem p3 = new ProductRangeItem()
            {
                Id = products.AsQueryable<Product>().Where(e => e.EngName == "Hot chocolate").Single().Id,
                Name = "Hot chocolate",
                Price = 2.5M,
                Category = ProductCategory.beverage_noncoffee
            };


            ProductRangeItem p4 = new ProductRangeItem()
            {
                Id = products.AsQueryable<Product>().Where(e => e.EngName == "skim milk").Single().Id,
                Name = "skim milk",
                Category = ProductCategory.milk_type
            };
            ProductRangeItem p5 = new ProductRangeItem()
            {
                Id = products.AsQueryable<Product>().Where(e => e.EngName == "soy milk").Single().Id,
                Name = "soy milk",
                Category = ProductCategory.milk_type
            };
            ProductRangeItem p6 = new ProductRangeItem()
            {
                Id = products.AsQueryable<Product>().Where(e => e.EngName == "whole milk").Single().Id,
                Name = "whole milk",
                Category = ProductCategory.milk_type
            };
            ProductRangeItem p7 = new ProductRangeItem()
            {
                Id = products.AsQueryable<Product>().Where(e => e.EngName == "2percent milk").Single().Id,
                Name = "2percent milk",
                Category = ProductCategory.milk_type
            };

            ProductRangeItem p8 = new ProductRangeItem()
            {
                Id = products.AsQueryable<Product>().Where(e => e.EngName == "whole bean french roast").Single().Id,
                Name = "whole bean french roast",
                Price = 3.5M,
                Category = ProductCategory.bagged_coffee
            };

            ProductRangeItem p9 = new ProductRangeItem()
            {
                Id = products.AsQueryable<Product>().Where(e => e.EngName == "whole bean light roast").Single().Id,
                Name = "whole bean light roast",
                Price = 3.5M,
                Category = ProductCategory.bagged_coffee
            };

            ProductRangeItem p10 = new ProductRangeItem()
            {
                Id = products.AsQueryable<Product>().Where(e => e.EngName == "espresso roast").Single().Id,
                Name = "espresso roast",
                Price = 3.1M,
                Category = ProductCategory.bagged_coffee
            };

            ProductRangeItem p11 = new ProductRangeItem()
            {
                Id = products.AsQueryable<Product>().Where(e => e.EngName == "vanilla").Single().Id,
                Name = "vanilla",
                Price = 0.5M,
                Category = ProductCategory.flavouring
            };

            ProductRangeItem p12 = new ProductRangeItem()
            {
                Id = products.AsQueryable<Product>().Where(e => e.EngName == "irish cream").Single().Id,
                Name = "irish cream",
                Price = 0.5M,
                Category = ProductCategory.flavouring
            };

            ProductRangeItem p13 = new ProductRangeItem()
            {
                Id = products.AsQueryable<Product>().Where(e => e.EngName == "caramel").Single().Id,
                Name = "caramel",
                Price = 0.5M,
                Category = ProductCategory.flavouring
            };

            ProductRangeItem p14 = new ProductRangeItem()
            {
                Id = products.AsQueryable<Product>().Where(e => e.EngName == "whipped cream").Single().Id,
                Name = "whipped cream",
                Price = 0.4M,
                Category = ProductCategory.extra_ingredient
            };

            ProductRangeItem p15 = new ProductRangeItem()
            {
                Id = products.AsQueryable<Product>().Where(e => e.EngName == "espresso").Single().Id,
                Name = "espresso",
                Price = 2,
                Category = ProductCategory.base_coffee
            };

            ProductRangeItem p16 = new ProductRangeItem()
            {
                Id = products.AsQueryable<Product>().Where(e => e.EngName == "brewed coffee").Single().Id,
                Name = "brewed coffee",
                Price = 2,
                Category = ProductCategory.base_coffee
            };

            ProductRangeItem p17 = new ProductRangeItem()
            {
                Id = products.AsQueryable<Product>().Where(e => e.EngName == "cocoa mix").Single().Id,
                Name = "cocoa mix",
                Category = ProductCategory.base_coffee
            };

            List<ProductRangeItem> lst = new List<ProductRangeItem>();
            lst.Add(p1);
            lst.Add(p2);
            lst.Add(p3);
            lst.Add(p4);
            lst.Add(p5);
            lst.Add(p6);
            lst.Add(p7);
            lst.Add(p8);
            lst.Add(p9);
            lst.Add(p10);
            lst.Add(p11);
            lst.Add(p12);
            lst.Add(p13);
            lst.Add(p14);
            lst.Add(p15);
            lst.Add(p16);
            lst.Add(p17);

            range.Products = lst;

            try
            {
                var coll = db.GetCollection<ProductRange>("productRanges");
                coll.InsertOne(range);
            }
            catch (Exception ex)
            {
                Error = "Error adding Us product range to the database: " + ex.Message;
                throw;
            }


        }

        public void AddStockItems()
        {

            var collStocks = db.GetCollection<Stock>("stocks");
            var locationCollection = db.GetCollection<Location>("locations");
            var collStockItems = db.GetCollection<StockItem>("stockitems");


            var usLocationId = locationCollection.AsQueryable().Where(e => e.Name == "US").Single().Id;
            var swedenLocationId = locationCollection.AsQueryable().Where(e => e.Name == "Sweden").Single().Id;


            Stock stockUS1 = new Stock()
            {
                StockName = "stockUS1",
                LocationId = usLocationId,
            };

            Stock stockSwe1 = new Stock()
            {
                StockName = "stockSwe1",
                LocationId = swedenLocationId,
            };

            collStocks.InsertOne(stockUS1);
            collStocks.InsertOne(stockSwe1);


            var collProducts = db.GetCollection<Product>("products");
            var allProducts = collProducts.Find(_ => true).ToEnumerable();

            List<StockItem> itemsUS = new List<StockItem>();
            List<StockItem> itemsSwe = new List<StockItem>();

            Random rand = new Random();

            foreach (var product in allProducts)
            {
                if (product.Category.ToString().Contains("beverage") == false)
                {
                    string unit = "decagram";
                    if (product.Category.Equals(ProductCategory.milk_type)
                        || product.Category.Equals(ProductCategory.extra_ingredient))
                    {
                        unit = "deciliter";
                    }
                    else if (product.Category.Equals(ProductCategory.flavouring))
                    {
                        unit = "milliliter";
                    }
                    else if (product.Category.Equals(ProductCategory.bagged_coffee))
                    {
                        unit ="kg";
                    }

                    StockItem i = new StockItem()
                    {
                        Amount = rand.Next(100, 500),
                        Unit = unit,
                        ProductId = product.Id,
                        StockId = stockUS1.Id
                    };

                    itemsUS.Add(i);


                    StockItem i2 = new StockItem()
                    {
                        Amount = rand.Next(100, 500),
                        Unit = unit,
                        ProductId = product.Id,
                        StockId = stockSwe1.Id
                    };

                    itemsSwe.Add(i2);
                }
            }

            try
            {
                collStockItems.InsertMany(itemsUS);
                collStockItems.InsertMany(itemsSwe);

            }
            catch (Exception ex)
            {
                Error = "Error adding stocks and stock items to the database: " + ex.Message;
                throw;
            }
        }

        public void AddCustomers()
        {

            var locationCollection = db.GetCollection<Location>("locations");
            var usLocationId = locationCollection.AsQueryable().Where(e => e.Name == "US").Single().Id;
            var swedenLocationId = locationCollection.AsQueryable().Where(e => e.Name == "Sweden").Single().Id;

            var collection = db.GetCollection<Employee>("employees");
            var employeeUS = collection.AsQueryable().Where(e => e.Positions.Any(p => p.WorkLocationId == usLocationId && p.EndDate < new DateTime(100, 01, 01, 00, 00, 00))).FirstOrDefault();
            var employeeSwe = collection.AsQueryable().Where(e => e.Positions.Any(p => p.WorkLocationId == swedenLocationId && p.EndDate < new DateTime(100, 01, 01, 00, 00, 00))).FirstOrDefault();


            List<Customer> customers = new List<Customer>();

            Customer cUs1 = new Customer()
            {
                Name = "Test Customer",
                PersonalId = "TCDFER34",
                Address = new Address()
                {
                    Country = "US",
                    City = "San Francisco",
                    Street = "Test street 345",
                    Zip = "34234"
                },
                LocationId = usLocationId,
                Occupation = Occupation.Student,
                RegisterDate = DateTime.UtcNow,
                RegisteringEmployeeId = employeeUS.Id
            };

            customers.Add(cUs1);


            Customer cSwe1 = new Customer()
            {
                Name = "Test Kund",
                PersonalId = "8512183265",
                Address = new Address()
                {
                    Country = "Sweden",
                    City = "Malmoe",
                    Street = "Test gatan 32",
                    Zip = "12334"
                },
                LocationId = swedenLocationId,
                Occupation = Occupation.Academy,
                RegisterDate = DateTime.UtcNow,
                RegisteringEmployeeId = employeeSwe.Id
            };

            customers.Add(cSwe1);

            try
            {
                var collCustomer = db.GetCollection<Customer>("customer");
                collCustomer.InsertMany(customers);
            }
            catch (Exception ex)
            {
                Error = "Error adding customers to the database: " + ex.Message;
                throw;
            }
        }
    }
}

using BeaverCoffeeDemo.Data;
using BeaverCoffeeDemo.ViewModels;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace BeaverCoffeeDemo
{
    /// <summary>
    /// Interaction logic for RegisterOrder.xaml
    /// </summary>
    public partial class RegisterOrder : UserControl, ISwitchable
    {
        private ObservableCollection<ProductRangeItem> _AllProducts = new ObservableCollection<ProductRangeItem>();
        private ObservableCollection<ProductViewModel> _OrderItems = new ObservableCollection<ProductViewModel>();

        private List<Employee> _ShopEmployees = new List<Employee>();
        private List<Customer> _LocationCustomers = new List<Customer>();
        private List<ProductOptionViewModel> _ProductAddOns = new List<ProductOptionViewModel>();
        private Order _Order = new Order();
        private ObjectId stockIdOfShop;

        public RegisterOrder()
        {
            InitializeComponent();
            GetShopEmployees();
            GetLocationCustomers();
            GetStockIdForCurrentLocation();
            GetProductRange();
            GetProductAdons();
            _OrderItems = new ObservableCollection<ProductViewModel>();
            _OrderItems.CollectionChanged += OrderItems_CollectionChanged;
            _Order.OrderItems = _OrderItems;

            this.DataContext = this;
        }

        public ObservableCollection<ProductRangeItem> Products
        { get { return _AllProducts; } }

        public ObservableCollection<ProductViewModel> OrderItems
        { get { return _OrderItems; } }

        public List<Employee> ShopEmployees
         { get { return _ShopEmployees; } }

        public List<Customer> LocationCustomers
        { get { return _LocationCustomers; } }

        public Order Order
        { get { return _Order; } }


        private void GetShopEmployees()
        {
            var employees = DbManager.GetInstance().GetCurrentEmployeesForShop(ShopGlobals.Shop.Id);
            if (employees != null)
            {
                _ShopEmployees = employees.ToList();
                cbShopEmployees.ItemsSource = ShopEmployees;
                if (ShopEmployees.Count >= 1)
                {
                    Employee currentEmployee = ShopEmployees.Where(e => e.Id == ShopGlobals.Employee.Id).SingleOrDefault();
                    if (currentEmployee != null)
                    {
                        int index = ShopEmployees.IndexOf(currentEmployee);
                        cbShopEmployees.SelectedIndex = index;

                    }
                    else
                        cbShopEmployees.SelectedIndex = 0;

                }
            }
        }

        private void GetLocationCustomers()
        {


            var customers = DbManager.GetInstance().GetCuctomersForLocation(ShopGlobals.Location.Id);
            if (customers != null)
            {
                _LocationCustomers = customers.ToList();
            }

            _LocationCustomers.Insert(0, new Customer()
            {
                Id = ObjectId.Empty,
                Name = "Unregistered"
            });

            cbLocationCustomers.ItemsSource = LocationCustomers;
            cbLocationCustomers.SelectedIndex = 0;
        }

        private void GetProductRange()
        {
            _AllProducts = new ObservableCollection<ProductRangeItem>((ShopGlobals.LocalProductRange.Products));
        }

        private void GetProductAdons()
        {
            var flavourings = _AllProducts.Where(p => p.Category.Equals(ProductCategory.flavouring));
            foreach (var item in flavourings)
            {
                ProductOptionViewModel vm = new ProductOptionViewModel();
                vm.Id = item.Id;
                vm.Name = item.Name;
                _ProductAddOns.Add(vm);
            }
        }

        private void GetStockIdForCurrentLocation()
        {
            stockIdOfShop = DbManager.GetInstance().GetStocksAtLocation(ShopGlobals.Location.Id).FirstOrDefault().Id;
        }
      

        public void UtilizeState(object state)
        {
            throw new NotImplementedException();
        }

        private void bntAddOrderItem_Click(object sender, RoutedEventArgs e)
        {
            ProductRangeItem selected = cbProducts.SelectedItem as ProductRangeItem;
            ProductViewModel vmSelected = ProductRangeItemToViewModel(selected);

            if (vmSelected.Ingredients != null && vmSelected.Ingredients.MilkTypeIngredients != null && vmSelected.Ingredients.MilkTypeIngredients.Options != null)
            {
                ProductOptionViewModel defaultMilkOption = vmSelected.Ingredients.MilkTypeIngredients.Options.ElementAt(0);
                vmSelected.Ingredients.MilkTypeIngredients.SelectedOption = defaultMilkOption;

                if (!CheckStockQuantitiesForProductOption(defaultMilkOption,vmSelected.Ingredients.MilkTypeIngredients.StockUnit))
                {
                    if (vmSelected.Ingredients.MilkTypeIngredients.Options.Count() > 1)
                    {
                        bool foundAvailableMilkOption = false;
                        int counter = 1;
                        while (!foundAvailableMilkOption && counter < vmSelected.Ingredients.MilkTypeIngredients.Options.Count())
                        {

                            ProductOptionViewModel milkOption = vmSelected.Ingredients.MilkTypeIngredients.Options.ElementAt(counter);
                            if (CheckStockQuantitiesForProductOption(milkOption, vmSelected.Ingredients.MilkTypeIngredients.StockUnit))
                            {
                                vmSelected.Ingredients.MilkTypeIngredients.SelectedOption = milkOption;
                                foundAvailableMilkOption = true;
                                MessageBox.Show("Product cannot be served with the default milk option: " + defaultMilkOption.Name);
                            }
                            counter++;
                        }
                    }

                }
            }

            vmSelected.Unit = 1;

            if (CheckStockQuantitiesForProduct(vmSelected))
            {
                _OrderItems.Add(vmSelected);
                dgBeverages.ItemsSource = OrderItems;
            }
        }

        private void cbMilkOptions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cb = sender as ComboBox;
            ProductOptionViewModel selected = cb.SelectedItem as ProductOptionViewModel;
            ProductViewModel rowItem = dgBeverages.SelectedItem as ProductViewModel;
            double unit = rowItem.Ingredients.MilkTypeIngredients.StockUnit;

            if (CheckStockQuantitiesForProductOption(selected, unit * rowItem.Unit))
            {
                rowItem.Ingredients.MilkTypeIngredients.SelectedOption = selected;
                dgBeverages.ItemsSource = null;
                dgBeverages.ItemsSource = OrderItems;
            }
        }

        private void cbExtraOptions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cb = sender as ComboBox;
            ProductOptionViewModel selected = cb.SelectedItem as ProductOptionViewModel;
            ProductViewModel rowItem = dgBeverages.SelectedItem as ProductViewModel;
            if (CheckStockQuantitiesForProductOption(selected, 1))
            {
                if (rowItem.AddOnList == null)
                {
                    rowItem.AddOnList = new List<ProductOptionViewModel>();
                }
                if (!rowItem.AddOnList.Contains(selected))
                {
                    rowItem.AddOnList.Add(selected);
                    ProductViewModel addOn = ProductRangeItemToViewModel(_AllProducts.Where(p => p.Id == selected.Id).Single());
                    addOn.AddedTo = rowItem.Id;
                    addOn.Unit = 1;
                    OrderItems.Add(addOn);
                    dgBeverages.ItemsSource = OrderItems;
                }
            }
        }

        private void btnRemoveItem_Click(object sender, RoutedEventArgs e)
        {
            if (dgBeverages.SelectedItem != null)
            {
                ProductViewModel selected = dgBeverages.SelectedItem as ProductViewModel;

                if (selected != null)
                {

                    if (OrderItems.Any(i => i.Id == selected.AddedTo))
                    {
                        ProductViewModel itemProductWasAddedTo = OrderItems.Where(i => i.Id == selected.AddedTo).Single();
                        ProductOptionViewModel addon = itemProductWasAddedTo.AddOnList.Where(a => a.Id == selected.Id).Single();
                        itemProductWasAddedTo.AddOnList.Remove(addon);
                    }

                    if (OrderItems.Any(i => i.AddedTo == selected.Id))
                    {
                        var addOnsToRemovedProuduct = OrderItems.Where(i => i.AddedTo == selected.Id);
                        List<ProductViewModel> toRemove = new List<ProductViewModel>();
                        foreach (var addon in addOnsToRemovedProuduct)
                        {
                            toRemove.Add(addon);
                        }

                        foreach (var item in toRemove)
                        {
                            OrderItems.Remove(item);
                        }
                    }

                    OrderItems.Remove(selected);
                    dgBeverages.ItemsSource = OrderItems;
                }
            }
        }


        private void OrderItems_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (INotifyPropertyChanged item in e.OldItems)
                    item.PropertyChanged -= item_PropertyChanged;
            }
            if (e.NewItems != null)
            {
                foreach (INotifyPropertyChanged item in e.NewItems)
                    item.PropertyChanged += item_PropertyChanged;
            }
            UpdateTotal();
        }


        private void item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Unit")
            {
                if (CheckStockQuantitiesForProduct((ProductViewModel)sender))
                {
                    UpdateTotal();
                }
                else
                {
                    ProductViewModel product = sender as ProductViewModel;
                    product.Unit = product.OriginalUnit;
                }
            }


        }

        private bool CheckStockQuantitiesForProduct(ProductViewModel product)
        {
            var db = DbManager.GetInstance();
            string message = string.Empty;
            List<Tuple<ObjectId, double>> stockQuantitiesWithUnits = StockIngredientsOfProduct(product);

            foreach (var pair in stockQuantitiesWithUnits)
            {
                if (!db.IsQuantityInStock(stockIdOfShop, pair.Item1, pair.Item2))
                {
                    message += "Not enough quantity in stock for " + ShopGlobals.LocalProductRange.Products.Where(p => p.Id == pair.Item1).FirstOrDefault().Name + " ";
                }
            }
            if (!string.IsNullOrEmpty(message))
            {
                MessageBox.Show(message);
                return false;
            }
            return true;
        }

        private bool CheckStockQuantitiesForProductOption(ProductOptionViewModel product, double unit)
        {
            var db = DbManager.GetInstance();
            string message = string.Empty;

            if (!db.IsQuantityInStock(stockIdOfShop, product.Id, unit))
            {
                MessageBox.Show("Not enough quantity in stock for " + ShopGlobals.LocalProductRange.Products.Where(p => p.Id == product.Id).FirstOrDefault().Name + " ");
                return false;
            }
            return true;
        }

        private void UpdateTotal()
        {
            decimal sum = 0;

            foreach (var item in OrderItems)
            {
                sum += item.Unit > 1 ? item.Price * item.Unit : item.Price;
            }
            Order.Total = sum;

            lblOrderTotal.Content = "Total: " + Order.Total + " " + ShopGlobals.LocalCurrency;
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new MainMenu());
        }

        private void btnCreateOrder_Click(object sender, RoutedEventArgs e)
        {
            ProcessOrder();
        }

        private void ProcessOrder()
        {
            if (CreateOrder())
            {
                string error = string.Empty;
                if (SaveOrder(out error))
                {
                    MessageBox.Show("Order has been processed");
                    Reset();
                }
                else
                {
                    MessageBox.Show(error);
                }
            }
        }

        private bool CreateOrder()
        {
            if (OrderItems != null && OrderItems.Count() > 0)
            {
                List<OrderItem> orderItems = new List<OrderItem>();
                foreach (var product in OrderItems)
                {
                    OrderItem item = ProductViewModelToOrderItem(product);
                    orderItems.Add(item);
                }

                Order.OrderItems = orderItems;
                Order.OrderDate = DateTime.UtcNow;
                Order.RegisteringEmployeeId = ((Employee)cbShopEmployees.SelectedItem).Id;
                Order.LocationId = ShopGlobals.Location.Id;
                Order.ShopId = ShopGlobals.Shop.Id;
                if (cbLocationCustomers.SelectedItem != null)
                {
                    Customer cust = cbLocationCustomers.SelectedItem as Customer;
                    if (cust.Id != ObjectId.Empty)
                        Order.CustomerId = cust.Id;

                    Order.Customer = new OrderCustomer()
                    {
                        Address = cust.Address,
                        Occupation = cust.Occupation,
                        Name = cust.Name
                    };
                }
                return true;
            }

            return false;
        }

        private bool SaveOrder(out string error)
        {
            error = string.Empty;
            if (DeduceOrderFromStock(out error))
            {
                if (DbManager.GetInstance().SaveOrder(Order))
                    return true;
            }
            error = DbManager.GetInstance().Error;
            return false;
        }

        private bool DeduceOrderFromStock(out string error)
        {
            error = string.Empty;
            List<TransactionItem> items = new List<TransactionItem>();

            foreach (var p in OrderItems)
            {
                List<Tuple<ObjectId, double>> stockQuantitiesWithUnits = StockIngredientsOfProduct(p);

                foreach(var pair in stockQuantitiesWithUnits)
                {
                    TransactionItem i = new TransactionItem()
                    {
                        ProductId = pair.Item1,
                        QuantityChange = 0 - pair.Item2,
                    };
                    items.Add(i);
                }
            }
            if (!DbManager.GetInstance().RegisterQuantityChangeInStock(items, stockIdOfShop))
            {
                error = DbManager.GetInstance().Error;
                return false;
            }
            return true;
        }

        private List<Tuple<ObjectId, double>> StockIngredientsOfProduct(ProductViewModel product)
        {
            List<Tuple<ObjectId, double>> ingredientIdsWithStockUnits = new List<Tuple<ObjectId, double>>();

            if (product.Category.ToString().Contains("beverage"))
            {
                // If it is a type of beverage, deduce constituting ingredients from stock
                if (product.Ingredients != null)
                {
                    if (product.Ingredients.BaseIngredients != null)
                    {
                        foreach (var i in product.Ingredients.BaseIngredients)
                        {
                            ingredientIdsWithStockUnits.Add(new Tuple<ObjectId, double>(i.Id, i.StockUnit * product.Unit));
                        }
                    }
                    if (product.Ingredients.MilkTypeIngredients != null)
                    {
                        ingredientIdsWithStockUnits.Add(new Tuple<ObjectId, double>(product.Ingredients.MilkTypeIngredients.SelectedOption.Id,
                            product.Ingredients.MilkTypeIngredients.StockUnit * product.Unit));
                    }
                }
            }
            else
            {
                ingredientIdsWithStockUnits.Add(new Tuple<ObjectId, double>(product.Id, product.Unit));
            }
            return ingredientIdsWithStockUnits;
        }

        private void Reset()
        {
            _Order = new Order();
            OrderItems.Clear();
            lblOrderTotal.Content = "Total: ";
            cbLocationCustomers.SelectedIndex = 0;
        }
        

        private ProductViewModel ProductRangeItemToViewModel(ProductRangeItem item)
        {
            ProductViewModel vm = new ProductViewModel();
            vm.Id = item.Id;
            vm.Name = item.Name;
            vm.Price = item.Price;
            vm.Category = item.Category;
            vm.Ingredients = new IngredientsViewModel();

            if (item.Category.ToString().Contains("beverage"))
            {
                vm.Ingredients.ExtraIngredients = _ProductAddOns;
            }

            if (item.Ingredients != null)
            {
                if (item.Ingredients.BaseIngredients != null && item.Ingredients.BaseIngredients.Count() > 0)
                {
                    List<BaseIngredientViewModel> vmBaseIngredientList = new List<BaseIngredientViewModel>();
                    foreach (var i in item.Ingredients.BaseIngredients)
                    {
                        BaseIngredientViewModel vmBaseIngredient = new BaseIngredientViewModel()
                        {
                            Id = i.Id,
                            StockUnit = i.StockUnit,
                            Name = _AllProducts.Where(p => p.Id == i.Id).Select(p => p.Name).Single(),
                        };
                        vmBaseIngredientList.Add(vmBaseIngredient);
                    }
                    vm.Ingredients.BaseIngredients = vmBaseIngredientList;
                }

                if (item.Ingredients.MilkTypeIngredients != null)
                {
                    MilkTypeIngredientsViewModel vmMilkType = new MilkTypeIngredientsViewModel()
                    {
                        StockUnit = item.Ingredients.MilkTypeIngredients.StockUnit,
                    };
                    vm.Ingredients.MilkTypeIngredients = vmMilkType;
                    List <ProductOptionViewModel> vmOptionList = new List<ProductOptionViewModel>();

                    foreach (var id in item.Ingredients.MilkTypeIngredients.Options)
                    {
                        ProductOptionViewModel vmOption = new ProductOptionViewModel()
                        {
                            Id = id,
                            Name = _AllProducts.Where(p => p.Id == id).Select(p => p.Name).Single(),
                        };
                        vmOptionList.Add(vmOption);
                    }

                    vm.Ingredients.MilkTypeIngredients.Options = vmOptionList;
                }

                if (item.Ingredients.ExtraIngredients != null && item.Ingredients.ExtraIngredients.Count() > 0)
                {

                    List<ProductOptionViewModel> vmOptionList = new List<ProductOptionViewModel>();

                    foreach (var id in item.Ingredients.ExtraIngredients)
                    {
                        ProductOptionViewModel vmOption = new ProductOptionViewModel()
                        {
                            Id = id,
                            Name = _AllProducts.Where(p => p.Id == id).Select(p => p.Name).Single(),
                        };
                        vmOptionList.Add(vmOption);
                        vmOptionList.AddRange(vm.Ingredients.ExtraIngredients);
                    }

                    vm.Ingredients.ExtraIngredients = vmOptionList;
                }
            }
            return vm;
        }

        private OrderItem ProductViewModelToOrderItem(ProductViewModel vm)
        {
            OrderItem item = new OrderItem()
            {
                Id = vm.Id,
                UnitAmount = vm.UnitAmount,
                Price = vm.Price,
            };

            if (vm.Ingredients != null && vm.Ingredients.MilkTypeIngredients != null && vm.Ingredients.MilkTypeIngredients.SelectedOption != null)
            {
                vm.MilkTypeChoiceId = vm.Ingredients.MilkTypeIngredients.SelectedOption.Id;
            }

            if (vm.AddOnList != null && vm.AddOnList.Count > 0)
            {
                item.AddOnIds = new List<ObjectId>();
                foreach (var addon in vm.AddOnList)
                {
                    item.AddOnIds.Add(addon.Id);
                }
            }

            return item;
        }
    }
}

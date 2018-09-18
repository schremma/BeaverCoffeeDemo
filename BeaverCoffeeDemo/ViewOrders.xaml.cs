using BeaverCoffeeDemo.Data;
using BeaverCoffeeDemo.ViewModels;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace BeaverCoffeeDemo
{
    /// <summary>
    /// Interaction logic for ViewOrders.xaml
    /// </summary>
    public partial class ViewOrders : UserControl, ISwitchable
    {
        private List<Location> _Locations;
        private List<OrderViewModel> _Orders;
        private List<Employee> _ShopEmployees = new List<Employee>();
        private bool showAllLocations = false;

        public ViewOrders()
        {
            InitializeComponent();
            SetAccessLevel();
            GetLocations();
            this.DataContext = this;
        }

        public List<Location> Locations
        { get { return _Locations; } }

        public List<OrderViewModel> Orders
        { get { return _Orders; } }

        public List<Employee> ShopEmployees
        { get { return _ShopEmployees; } }

        private void GetLocations()
        {
            var locations = DbManager.GetInstance().GetAllLocations().ToList();

            if (!showAllLocations)
            {
                var locationIds = ShopGlobals.Employee.Positions.Where(p => p.EndDate < new DateTime(100, 01, 01, 00, 00, 00)).Select(p => p.WorkLocationId);
                locations.RemoveAll(l => !locationIds.Contains(l.Id));
            }
            _Locations = locations;
            if (showAllLocations)
            {
                _Locations.Insert(0, new Location()
                {
                    Name = "All locations",
                    Id = ObjectId.Empty,
                });
            }
        }

        private void GetShopEmployees(Location location)
        {
            cbShopEmployees.ItemsSource = null;
            _ShopEmployees.Clear();
            _ShopEmployees.Insert(0, new Employee() { Name = "All employees" });
            if (location.Id != ObjectId.Empty)
            {
                var employees = DbManager.GetInstance().GetShopEmployeesForLocation(location);
                if (employees != null)
                {
                    _ShopEmployees.AddRange(employees.ToList());
                }
            }
            else
            {
                var employees = DbManager.GetInstance().GetAllShopEmployees();
                if (employees != null)
                {
                    _ShopEmployees.AddRange(employees.ToList());
                }
            }
            cbShopEmployees.ItemsSource = ShopEmployees;
            if (ShopEmployees != null && ShopEmployees.Count >= 1)
                cbShopEmployees.SelectedIndex = 0;
        }

        private void GetOrders(ObjectId locationId)
        {
            if (locationId == ObjectId.Empty)
            {
                var orders = DbManager.GetInstance().GetAllOrders();
                LoadOrders(orders);
            }
            else
            {
                var orders = DbManager.GetInstance().GetOrdersForLocation(locationId);
                LoadOrders(orders);
            }
 
        }

        private void GetOrders(ObjectId locationId, ObjectId employeeId)
        {
            if (locationId == ObjectId.Empty)
            {
                var orders = DbManager.GetInstance().GetOrdersPerEmployee(employeeId);
                LoadOrders(orders);
            }
            else
            {
                var orders = DbManager.GetInstance().GetOrdersForLocationPerEmployee(locationId, employeeId);
                LoadOrders(orders);
            }
        }

        private void GetOrdersForDates(ObjectId locationId, DateTime start, DateTime end)
        {
            if (locationId != ObjectId.Empty)
            {
                var orders = DbManager.GetInstance().GetOrdersInDateIntervalForLocation(locationId, start, end);
                LoadOrders(orders);
            }
            else
            {
                var orders = DbManager.GetInstance().GetAllOrdersInDateInterval(start, end);
                LoadOrders(orders);
            }
        }

        private void GetOrdersForDates(ObjectId locationId, ObjectId employeeId, DateTime start, DateTime end)
        {
            var orders = DbManager.GetInstance().GetOrdersInDateIntervalForLocationPerEmployee(locationId, employeeId, start, end);

            LoadOrders(orders);
        }

        private void LoadOrders(IEnumerable<Order> orders)
        {
            if (Orders != null)
            {
                dgOrders.ItemsSource = null;
                Orders.Clear();
            }
            else
                _Orders = new List<OrderViewModel>();
            foreach (var order in orders)
            {
                Orders.Add(OrderToViewModel(order));
            }
            dgOrders.ItemsSource = Orders;
        }

        private void SetAccessLevel()
        {
            int currentHighestPosition = 0;
            if (ShopGlobals.Employee.Positions != null)
                currentHighestPosition = ShopGlobals.Employee.Positions.Where(p => p.EndDate < new DateTime(100, 01, 01, 00, 00, 00)).Select(p => p.Position).Max();
            showAllLocations = DbManager.GetInstance().IsCorporateSalesManager(currentHighestPosition);
        }

        private OrderViewModel OrderToViewModel(Order order)
        {
            OrderViewModel vm = new OrderViewModel();

            Currency currency = DbManager.GetInstance().CurrencyForLocation(order.LocationId);
            vm.Date = order.OrderDate;
            vm.TotalString = order.Total + " " + currency.ToString();
            vm.OrderId = order.Id;
            if (order.Customer != null && order.CustomerId != ObjectId.Empty)
            {
                vm.CustomerDetails = order.Customer.ToString();
            }
            else
                vm.CustomerDetails = "Unregistered customer";

            StringBuilder productDetails = new StringBuilder();
            foreach (var item in order.OrderItems)
            {
                productDetails.Append(ShopGlobals.LocalProductRange.Products.Where(p => p.Id == item.Id).FirstOrDefault().Name);
                productDetails.Append(", ");
                productDetails.Append("unit: ");
                productDetails.Append(item.UnitAmount);
                productDetails.Append(", ");
                productDetails.Append("price: ");
                productDetails.Append(item.Price);
                productDetails.Append("; ");
            }
            vm.ProductDetails = productDetails.ToString();

            return vm;
        }

        public void UtilizeState(object state)
        {
            throw new NotImplementedException();
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new MainMenu());
        }

        private void cbLocations_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Location selected = cbLocations.SelectedItem as Location;
            if (selected != null)
            {
                GetOrders(selected.Id);
                GetShopEmployees(selected);
                datePickerEnd.IsEnabled = true;
                datePickerStart.IsEnabled = true;
                btnFilterByDate.IsEnabled = true;
            }
        }

        private void btnFilterByDate_Click(object sender, RoutedEventArgs e)
        {
            if (datePickerStart.SelectedDate.HasValue && datePickerEnd.SelectedDate.HasValue)
            {
                DateTime start = datePickerStart.SelectedDate.Value;
                DateTime end = datePickerEnd.SelectedDate.Value;

                if (end >= start)
                {
                    Employee selectedEmployee = cbShopEmployees.SelectedItem as Employee;
                    Location loc = cbLocations.SelectedItem as Location;
                    if (selectedEmployee != null && selectedEmployee.Id != ObjectId.Empty)
                    {
                        GetOrdersForDates(loc.Id, selectedEmployee.Id, start, end);
                    }
                    else if (loc != null)
                    {
                        GetOrdersForDates(loc.Id, start, end);
                    }
                }
                else
                {
                    MessageBox.Show("End date must be equal to or larger than start date");
                }
            }
            else
            {
                MessageBox.Show("Select both a start and an end date");
            }
        }

        private void cbShopEmployees_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Employee employee = cbShopEmployees.SelectedItem as Employee;
            Location loc = cbLocations.SelectedItem as Location;
            if (employee != null && employee.Id != ObjectId.Empty)
            {
                GetOrders(loc.Id, employee.Id);
            }
            else if (loc != null && loc.Id != ObjectId.Empty)
            {
                GetOrders(loc.Id);
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (dgOrders.SelectedItem != null)
            {
                OrderViewModel selected = dgOrders.SelectedItem as OrderViewModel;
                if (selected != null)
                {
                    RemoveOrder(selected);
                }
            }
        }

        private void RemoveOrder(OrderViewModel order)
        {
            if (DbManager.GetInstance().DeleteOrder(order.OrderId))
            {
                Orders.Remove(order);
                dgOrders.ItemsSource = null;
                dgOrders.ItemsSource = Orders;
            }
            else
            {
                MessageBox.Show("Deleting order failed");
            }

        }
    }
}

using BeaverCoffeeDemo.Data;
using BeaverCoffeeDemo.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace BeaverCoffeeDemo
{
    /// <summary>
    /// Interaction logic for Start.xaml
    /// </summary>
    public partial class Start : UserControl, ISwitchable
    {
        private List<Location> _Locations;
        private List<Employee> _Employees = new List<Employee>();

        private const String BUTTON_NAME_INSERT_DEMO_DATA = "Insert demo data";
        private const String BUTTON_NAME_RESET_DEMO_DATA = "Reset demo data";

        public Start()
        {
            InitializeComponent();
            GetLocations();
        }

        public List<Location> Locations
        { get { return _Locations; } }

        public List<Employee> Employees
        { get { return _Employees; } }

        private void GetLocations()
        {
            var locations = DbManager.GetInstance().GetAllLocations();
            if (locations == null || locations.Count() == 0)
            {
                //Need to add some data to the database first
                cbLocation.IsEnabled = false;
                btnSeedDb.Content = BUTTON_NAME_INSERT_DEMO_DATA;
            }
            else
            {
                _Locations = locations.ToList();
                cbLocation.ItemsSource = Locations;
                btnSeedDb.Content = BUTTON_NAME_RESET_DEMO_DATA;
            }

        }

        public void UtilizeState(object state)
        {
            throw new NotImplementedException();
        }

        private void GetProductRange()
        {
            DbManager db = DbManager.GetInstance();
            ProductRange range = db.GetProductRangeForLocation(ShopGlobals.Location);
            var products = db.GetAllProductsWithIngredients();

            foreach (var item in range.Products)
            {
                if (item.Category.Equals(ProductCategory.beverage_coffee) || item.Category.Equals(ProductCategory.beverage_noncoffee))
                {
                    var ingredients = products.Where(p => p.Id == item.Id).Select(p => p.Ingredients).Single();
                    item.Ingredients = ingredients;
                }
            }

            ShopGlobals.LocalProductRange = range;
        }

        private void btnAddCustomer_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new RegisterCustomer());

        }

        private void btnSeedDb_Click(object sender, RoutedEventArgs e)
        {
            Data.SeedDb seed = new Data.SeedDb();
            if (seed.Seed())
            {
                MessageBox.Show("Test data has been added to the database");
                cbLocation.IsEnabled = true;
                ReloadDefaultOptions();
            }
            else
            {
                MessageBox.Show("Error adding test data to database: " + seed.Error);
            }

        }

        private void ReloadDefaultOptions()
        {
            GetLocations();
        }


        private void cbLocation_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Location location = cbLocation.SelectedItem as Location;
            if (location != null)
            {
                ShopGlobals.Location = location;

                stackEmployeeType.IsEnabled = true;
                cbEmployees.IsEnabled = true;
                cbShops.IsEnabled = true;
                btnToMenu.IsEnabled = true;

                ShopGlobals.LocalCurrency = DbManager.GetInstance().CurrencyForLocation(location.Id);
                GetProductRange();
                LoadShopList();
            }
        }

        private void LoadShopList()
        {
            List<ShopViewModel> shopList = new List<ShopViewModel>();
            foreach (var s in ShopGlobals.Location.Shops)
            {
                shopList.Add(ShopToViewModel(s));
            }
            cbShops.ItemsSource = shopList;
            if (shopList.Count > 0)
            {
                cbShops.SelectedIndex = 0;
                SetShop(shopList[0]);
            }
        }

        private void SetShop(ShopViewModel selected)
        {
            ShopGlobals.Shop = selected;
            LoadEmployees();
        }

        private void LoadEmployees()
        {
            if (cbEmployees != null)
            {
                IEnumerable<Employee> employees;
                if (rbShopEmployee.IsChecked.HasValue && rbShopEmployee.IsChecked.Value == true)
                {
                    employees = DbManager.GetInstance().GetCurrentEmployeesForShop(ShopGlobals.Shop.Id);
                }
                else if (rbManager.IsChecked.HasValue && rbManager.IsChecked.Value == true)
                {
                    employees = DbManager.GetInstance().GetCurrentLocationManagers(ShopGlobals.Location);
                }
                else
                {
                    employees = DbManager.GetInstance().GetCurrentCorporateManagers();
                }
                if (employees != null)
                {
                    _Employees = employees.ToList();
                    cbEmployees.ItemsSource = Employees;
                    if (Employees.Count > 0)
                    {
                        cbEmployees.SelectedIndex = 0;
                        SetEmployee(Employees[0]);
                    }

                }
            }
        }

        private void SetEmployee(Employee employee)
        {
            ShopGlobals.Employee = employee;
        }


        private ShopViewModel ShopToViewModel(Shop shop)
        {
            ShopViewModel vm = new ShopViewModel()
            {
                Id = shop.Id,
                ShopDescription = shop.Address.ToString()
            };
            return vm;
        }

        private void cbShops_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ShopViewModel selected = cbShops.SelectedItem as ShopViewModel;
            if (selected != null)
            {
                SetShop(selected);
            }

        }

        private void cbEmployees_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Employee selected = cbEmployees.SelectedItem as Employee;
            if (selected != null)
            {
                SetEmployee(selected);
            }
        }

        private void rbShopEmployee_Checked(object sender, RoutedEventArgs e)
        {
            if (ShopGlobals.Shop != null)
            {
                LoadEmployees();
            }

        }

        private void rbManager_Checked(object sender, RoutedEventArgs e)
        {
            if (ShopGlobals.Location != null)
            {
                LoadEmployees();
            }

        }


        private void rbCorporateManager_Checked(object sender, RoutedEventArgs e)
        {
            if (ShopGlobals.Location != null)
            {
                LoadEmployees();
            }
        }

        private void btnToMenu_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new MainMenu());
        }

    }
}

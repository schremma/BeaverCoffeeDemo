using BeaverCoffeeDemo.Data;
using BeaverCoffeeDemo.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BeaverCoffeeDemo
{
    /// <summary>
    /// Interaction logic for MainMenu.xaml
    /// </summary>
    public partial class MainMenu : UserControl, ISwitchable
    {


        public MainMenu()
        {
            InitializeComponent();
            SetAccessLevel();
            TestShopInfo();
    
        }

        private void SetAccessLevel()
        {
            var db = DbManager.GetInstance();

            if (ShopGlobals.Employee.Positions != null && ShopGlobals.Employee.Positions.Any(p => p.BossOver != null))
            {
                btnComment.IsEnabled = true;
            }
            else
            {
                btnComment.IsEnabled = false;
            }

            int currentHighestPosition = 0;
            if (ShopGlobals.Employee.Positions != null)
                currentHighestPosition = ShopGlobals.Employee.Positions.Where(p => p.EndDate < new DateTime(100, 01, 01, 00, 00, 00)).Select(p => p.Position).Max();
            if (!db.IsManager(currentHighestPosition))
            {
                btnAnalytics.IsEnabled = false;
                btnEmployeeList.IsEnabled = false;
                btnManageStockItmes.IsEnabled = false;
                btnViewOrders.IsEnabled = false;             
            }
        }

        public void UtilizeState(object state)
        {
            throw new NotImplementedException();
        }

        private void TestShopInfo()
        {
            StringBuilder info = new StringBuilder();
            info.Append("Location: ");
            info.Append(ShopGlobals.Location.Name);
            info.Append(". ");
            info.Append("Coffee shop: ");
            info.Append(ShopGlobals.Shop.ShopDescription);
            info.Append(". ");
            info.Append("Employee: ");
            info.Append(ShopGlobals.Employee.Name);
            info.Append(". ");

            lblShop.Content = info.ToString();
        }


        private void btnAddCustomer_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new RegisterCustomer());

        }


        private void btnProductRange_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new ProductRangeList());
        }

        private void btnOrder_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new RegisterOrder());
        }

        private void btnEmployeeList_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new EmployeeList());
        }

        private void btnViewOrders_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new ViewOrders());
        }

        private void btnManageStockItmes_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new ManageStocks());
        }

        private void btnAnalytics_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new AnalyticsView());
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new Start());
        }

        private void btnComment_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new AddComment());
        }
    }
}

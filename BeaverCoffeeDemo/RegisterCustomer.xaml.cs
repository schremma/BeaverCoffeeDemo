using BeaverCoffeeDemo.Data;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace BeaverCoffeeDemo
{
    /// <summary>
    /// Interaction logic for RegisterCustomer.xaml
    /// </summary>
    public partial class RegisterCustomer : UserControl, ISwitchable
    {
        private Customer _customer;
        private string message;

        public RegisterCustomer()
        {
            InitializeComponent();

            initCustomer();
            this.DataContext = _customer;
        }

        private void initCustomer()
        {
            _customer = new Customer();
            _customer.Address = new Address();
            _customer.Address.Country = ShopGlobals.Location.Name;
        }

        public Customer Customer
        { get { return _customer; } }


        public void UtilizeState(object state)
        {

        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            txtName.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            txtPersonalId.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            txtStreet.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            cbOccupations.ItemsSource = Enum.GetValues(typeof(Occupation)).Cast<Occupation>();
            cbOccupations.SelectedIndex = 0;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!System.Windows.Controls.Validation.GetHasError(txtName) && !System.Windows.Controls.Validation.GetHasError(txtPersonalId)
                && !System.Windows.Controls.Validation.GetHasError(txtStreet) && !System.Windows.Controls.Validation.GetHasError(txtZip))
            {
                Customer.RegisterDate = DateTime.Now;
                Customer.Occupation = (Occupation)cbOccupations.SelectedItem;
                Customer.RegisteringEmployeeId = ShopGlobals.Employee.Id;
                Customer.LocationId = ShopGlobals.Location.Id;
                if (saveCustomerInDb(Customer))
                {
                    MessageBox.Show("Customer has been added");
                    _customer = new Customer();
                    this.DataContext = _customer;
                }
                else
                {
                    MessageBox.Show(message);
                }
            }
            else
            {
                MessageBox.Show("Fill in all the fields with valid values.");
            }
        }

        private bool saveCustomerInDb(Customer customer)
        {
            DbManager db = DbManager.GetInstance();
            if (db.AddCustomer(customer))
            {
                return true;
            }
            else
            {
                message = db.Error != null ? db.Error : "";
                return false;
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new MainMenu());
        }
    }
}

using BeaverCoffeeDemo.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BeaverCoffeeDemo
{
    /// <summary>
    /// Interaction logic for ProductRange.xaml
    /// </summary>
    public partial class ProductRangeList : UserControl, ISwitchable
    {
        private ObservableCollection<ProductRangeItem> _Products = new ObservableCollection<ProductRangeItem>();

        public ProductRangeList()
        {
            InitializeComponent();
            GetProductRange();
            this.DataContext = this;
        }

        public ObservableCollection<ProductRangeItem> Products
        { get { return _Products; } }

        private void GetProductRange()
        {
            _Products = new ObservableCollection<ProductRangeItem>(ShopGlobals.LocalProductRange.Products);
        }

        public void UtilizeState(object state)
        {
            throw new NotImplementedException();
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new MainMenu());
        }
    }
}

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
    /// Interaction logic for ManageStocks.xaml
    /// </summary>
    public partial class ManageStocks : UserControl
    {
        private List<Stock> _Stocks;
        private ObservableCollection<StockItemViewModel> _StockItems;
        private ObjectId _CurrentStockId;
        private bool showAllLocations;
        private bool resetAmount = false;

        public ManageStocks()
        {
            InitializeComponent();
            SetAccessLevel();
            GetStocks();
            this.DataContext = this;
        }



        public List<Stock> Stocks
        { get { return _Stocks; } }

        public ObservableCollection<StockItemViewModel> StockItems
        { get { return _StockItems; } }

        private void GetStocks()
        {
            var db = DbManager.GetInstance();
            List<Stock> stocks = new List<Stock>();
            if (showAllLocations)
            {
                var locations = db.GetAllLocations();
                foreach (var l in locations)
                {
                    var stocksAtLocation = db.GetStocksAtLocation(l.Id);
                    if (stocksAtLocation != null)
                        stocks.AddRange(stocksAtLocation);
                }
            }
            else
                stocks = db.GetStocksAtLocation(ShopGlobals.Location.Id).ToList();

            _Stocks = stocks;
            cbStocks.ItemsSource = Stocks;
        }

        private void GetStockItems(Stock stock)
        {
            var items = DbManager.GetInstance().GetStockItemsInStock(stock.Id);
            
            if (StockItems != null)
            {
                StockItems.Clear();
                dgStockItems.Items.Refresh();
            }
            else
                _StockItems = new ObservableCollection<StockItemViewModel>();
            StockItems.CollectionChanged += StockItems_CollectionChanged;
            foreach (var item in items)
            { 

                StockItemViewModel stockVm = StockItemToViewModel(item);
                StockItems.Add(stockVm);
            }
            dgStockItems.ItemsSource = StockItems;
        }

        private void StockItems_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (INotifyPropertyChanged item in e.OldItems)
                {
                    item.PropertyChanged -= Item_PropertyChanged;
                }
            }
            if (e.NewItems != null)
            {
                foreach (INotifyPropertyChanged item in e.NewItems)
                {
                    item.PropertyChanged += Item_PropertyChanged;
                }
            }
        }

        private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("Amount"))
            {
                StockItemViewModel item = sender as StockItemViewModel;
                if (item != null)
                {
                    UpdateStockItemAmount(item);
                }
            }
        }

        private void UpdateStockItemAmount(StockItemViewModel item)
        {
            if (!resetAmount)
            {
                if (item.Amount >= 0)
                {
                    List<TransactionItem> items = new List<TransactionItem>();
                    TransactionItem i = new TransactionItem()
                    {
                        ProductId = item.ProductId,
                        QuantityChange = item.Amount - item.StartingAmount
                    };
                    items.Add(i);
                    if (DbManager.GetInstance().RegisterQuantityChangeInStock(items, _CurrentStockId))
                    {
                        MessageBox.Show("Updated stock item in database");
                    }
                    else
                        MessageBox.Show("Could not update stock item in database");
                }
                else
                {
                    resetAmount = true;
                    item.Amount = item.StartingAmount;
                    MessageBox.Show("Please enter a positive value");
                }
            }
            else
            {
                resetAmount = false;
            }
        }


        private void SetAccessLevel()
        {
            int currentHighestPosition = 0;
            if (ShopGlobals.Employee.Positions != null)
                currentHighestPosition = ShopGlobals.Employee.Positions.Where(p => p.EndDate < new DateTime(100, 01, 01, 00, 00, 00)).Select(p => p.Position).Max();
            showAllLocations = DbManager.GetInstance().IsCorporateSalesManager(currentHighestPosition);
        }


        public void UtilizeState(object state)
        {
            throw new NotImplementedException();
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new MainMenu());
        }


        private void cbStocks_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Stock selected = cbStocks.SelectedItem as Stock;
            if (selected != null)
            {
                _CurrentStockId = selected.Id;
                GetStockItems(selected);
            }

        }

        private StockItemViewModel StockItemToViewModel(StockItem item)
        {
            string name = ShopGlobals.LocalProductRange.Products.Where(p => p.Id == item.ProductId).FirstOrDefault().Name;
            StockItemViewModel vm = new StockItemViewModel()
            {
                Amount = item.Amount,
                StartingAmount = item.Amount,
                Id = item.Id,
                ProductId = item.ProductId,
                ProductName = name,
                Unit = item.Unit
            };

            return vm;
        }
    }
}

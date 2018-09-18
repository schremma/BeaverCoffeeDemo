using BeaverCoffeeDemo.Data;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace BeaverCoffeeDemo
{
    /// <summary>
    /// Interaction logic for AnalyticsView.xaml
    /// </summary>
    public partial class AnalyticsView : UserControl, ISwitchable
    {
        private List<Location> _Locations;
        private bool showAllLocations = false;

        public AnalyticsView()
        {
            InitializeComponent();
            SetAccessLevel();
            GetLocations();
            GetStockItems();
            this.DataContext = this;
            InitGui();

        }

        public List<Location> Locations
        { get { return _Locations; } }

        private void InitGui()
        {
            datePickerEnd.SelectedDate = DateTime.Now.AddDays(1);
            datePickerStart.SelectedDate = new DateTime(DateTime.Now.Year, 01, 01, 00, 00, 00);

            cbAction.Items.Add(new Tuple<Analytics, string>(Analytics.Sales, "1. Sales for time period"));
            cbAction.Items.Add(new Tuple<Analytics, string>(Analytics.StockTransactions, "4. Stock quantities of product"));
            cbAction.SelectedIndex = 0;
            cbStockItems.Visibility = Visibility.Hidden;
        }


        private void ShowAnalytics<T>(List<string> columns, List<T> itemList)
        {
            ClearAnalytics();

            for (int i = 0; i < columns.Count; i++)
            {
                string bindingProperty = "Item" + (i+1);
                dgAnalytics.Columns.Add(new DataGridTextColumn()
                {
                    Header = columns[i],
                    Binding = new Binding(bindingProperty),
                });
            }


            foreach (var item in itemList)
            {
                dgAnalytics.Items.Add(item);
            }

        }

        private void ClearAnalytics()
        {
            dgAnalytics.Items.Clear();
            dgAnalytics.Columns.Clear();
        }

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

            cbLocations.ItemsSource = _Locations;
            if (cbLocations.Items != null && cbLocations.Items.Count > 0)
            {
                cbLocations.SelectedIndex = 0;
            }
        }

        private void GetStockItems()
        {
            Location selectedLocation = cbLocations.SelectedItem as Location;
            if (selectedLocation != null)
            {
                if (cbStockItems.Items != null)
                    cbStockItems.Items.Clear();

                var db = DbManager.GetInstance();

                List<Stock> stocks = new List<Stock>();

                if (selectedLocation.Id != ObjectId.Empty)
                {
                    var stocksAtLocation = db.GetStocksAtLocation(selectedLocation.Id);
                    if (stocksAtLocation != null)
                        stocks.AddRange(stocksAtLocation);
                }
                else
                {
                    // get all stock items in all stocks
                    var locations = db.GetAllLocations();
                    foreach (var l in locations)
                    {
                        var stocksAtLocation = db.GetStocksAtLocation(l.Id);
                        if (stocksAtLocation != null)
                            stocks.AddRange(stocksAtLocation);
                    }
                }
                List<StockItem> stockItems = new List<StockItem>();
                foreach (var stock in stocks)
                {
                    var items = db.GetStockItemsInStock(stock.Id);
                    if (items != null)
                        stockItems.AddRange(items);
                }

                var distinctList = stockItems.GroupBy(x => x.ProductId).Select(g => g.First()).ToList();

                foreach (var item in distinctList)
                {
                    string name = db.EnglishNameOfProduct(item.ProductId);
                    cbStockItems.Items.Add(new ProductItem { ProductId = item.ProductId, Name = name });
                }
                if (cbStockItems.Items.Count > 0)
                    cbStockItems.SelectedIndex = 0;
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

        private void btnCalculateSalesStatistics_Click(object sender, RoutedEventArgs e)
        {
            Analytics selectedAction = GetSelectedAction();

            switch (selectedAction)
            {
                case Analytics.Sales: GetSales();
                    break;
                case Analytics.StockTransactions: GetStockTransactionsForProduct();
                    break;
            }
            
        }

        private void GetSales()
        {
            Location selectedLocation = cbLocations.SelectedItem as Location;
            if (selectedLocation != null)
            {
                if (datePickerStart.SelectedDate.HasValue && datePickerEnd.SelectedDate.HasValue)
                {
                    DateTime start = datePickerStart.SelectedDate.Value;
                    DateTime end = datePickerEnd.SelectedDate.Value;

                    if (selectedLocation.Id != ObjectId.Empty)
                    {
                        var orders = DbManager.GetInstance().GetOrdersInInterval(selectedLocation.Id, start, end);
                        CreateSalesAnalyticsForInterval(orders);
                    }
                    else
                    {
                        var orders = DbManager.GetInstance().GetAllOrdersInInterval(start, end);
                        CreateSalesAnalyticsForInterval(orders);
                    }
                }
                else
                {
                    MessageBox.Show("Select a valid date interval");
                }
            }
            else
                MessageBox.Show("Select location specification first");
        }

        private void CreateSalesAnalyticsForInterval(IEnumerable<Order> orders)
        {
            
            var items = orders.SelectMany(o => o.OrderItems).
                GroupBy(i => i.Id, 
                        i => new { Total = i.Price * i.UnitAmount, Unit = i.UnitAmount }, 
                        (key, g) => new { ProductId = key, Totals = new { TotalPrice = g.Sum(i => i.Total), TotalUnit = g.Sum(i => i.Unit)} });


            List<string> columns = new List<string>();
            columns.Add("Product");
            columns.Add("Units sold");
            columns.Add("Total");

            List<Tuple<string, string, string>> itemsList = new List<Tuple<string, string, string>>();
            foreach (var i in items)
            {
                string name = ShopGlobals.LocalProductRange.Products.Where(p => p.Id == i.ProductId).FirstOrDefault().Name;
                itemsList.Add(new Tuple<string, string, string>(name, i.Totals.TotalUnit. ToString(), i.Totals.TotalPrice.ToString()));
            }

            ShowAnalytics(columns, itemsList);
            
        }

        private void GetStockTransactionsForProduct()
        {
            Location selectedLocation = cbLocations.SelectedItem as Location;
            ProductItem product = cbStockItems.SelectedItem as ProductItem;
            if (selectedLocation != null && product != null)
            {
                if (datePickerStart.SelectedDate.HasValue && datePickerEnd.SelectedDate.HasValue)
                {
                    DateTime start = datePickerStart.SelectedDate.Value;
                    DateTime end = datePickerEnd.SelectedDate.Value;

                    if (selectedLocation.Id != ObjectId.Empty)
                    {
                        var items = DbManager.GetInstance().GetStockTransactionsForProductAtLocationAndInterval(selectedLocation.Id, product.ProductId,
                            start, end);
                        if (items != null)
                            CreateStockReportForProduct(items);
                        else
                        {
                            TransactionItem lastTransaction = DbManager.GetInstance().GetLatestStockTransactionForProductAtLocation(selectedLocation.Id, product.ProductId);
                            string message = "";
                            if (lastTransaction != null)
                            {
                                message = "Latest stock quanity was: " + lastTransaction.StartingStockAmount + " at " + lastTransaction.TimeStamp;
                            }
                            ClearAnalytics();
                            MessageBox.Show("No stock transactions took place for product in interval. " + message);

                        }
                    }
                    else
                    {
                        var items = DbManager.GetInstance().GetStockTransactionsForProductInInterval(product.ProductId,
                            start, end);
                        if (items != null)
                            CreateStockReportForProduct(items);
                        else
                        {
                            TransactionItem lastTransaction = DbManager.GetInstance().GetLatestStockTransactionForProduct(product.ProductId);
                            string message = "";
                            if (lastTransaction != null)
                            {
                                message = "Latest stock quanity was: " + lastTransaction.StartingStockAmount + " at " + lastTransaction.TimeStamp;
                            }
                            ClearAnalytics();
                            MessageBox.Show("No stock transactions took place for product in interval. " + message);
                            
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Select a valid date interval");
                }
            }
            else
                MessageBox.Show("Select location specification and product name first");
        }

        private void CreateStockReportForProduct(IEnumerable<Tuple<ObjectId, IEnumerable<TransactionItem>>> transactionsPerStockPerDay)
        {
            if (transactionsPerStockPerDay != null && transactionsPerStockPerDay.Count() > 0)
            {
                List<string> columns = new List<string>();
                columns.Add("Stock name");
                columns.Add("Starting amount");
                columns.Add("Units added");
                columns.Add("Units deduced");
                columns.Add("Final amount");

                List<Tuple<string, string, string, string, string>> itemsList = new List<Tuple<string, string, string, string, string>>();
                // Transactions are listed by stock

                //var transactionsPerStock = transactionsPerStockPerDay.GroupBy(t => t.Item1, t => t.Item2, 
                //    (key, g) => new Tuple<ObjectId, IEnumerable<TransactionItem>>(key, g));

                var transactionsPerStock = transactionsPerStockPerDay.GroupBy(t => t.Item1).Select(group => new
                 Tuple<ObjectId, IEnumerable<TransactionItem>>(
                    group.Key,
                    group.SelectMany(item => item.Item2).ToList()
                )).ToList();

                foreach (var stock in transactionsPerStock)
                {
                    string stockName = DbManager.GetInstance().GetStockNameForId(stock.Item1);

                    var transactions = stock.Item2;

                    double startingStockAmount = transactions.OrderByDescending(t => t.TimeStamp).LastOrDefault().StartingStockAmount;
                    double unitsAdded = transactions.Where(t => t.QuantityChange > 0).Select(t => t.QuantityChange).Sum();
                    double unitsDeduced = transactions.Where(t => t.QuantityChange < 0).Select(t => t.QuantityChange).Sum();
                    TransactionItem final = transactions.OrderByDescending(t => t.TimeStamp).FirstOrDefault();
                    double finalStockAmount = final.StartingStockAmount + final.QuantityChange;


                    itemsList.Add(new Tuple<string, string, string, string, string>(stockName,
                        startingStockAmount.ToString(),
                        unitsAdded.ToString(),
                        unitsDeduced.ToString(),
                        finalStockAmount.ToString()));
                }

                ShowAnalytics(columns, itemsList);
            }
            else
            {
                ClearAnalytics();
                MessageBox.Show("No stock transactions took place for product in interval");
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new MainMenu());
        }

        private void cbLocations_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GetStockItems();
        }

        private void cbAction_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Analytics selectedAction = GetSelectedAction();
            if (!selectedAction.Equals(Analytics.None))
            {
                if (selectedAction.Equals(Analytics.Sales))
                    cbStockItems.Visibility = Visibility.Hidden;
                else if (selectedAction.Equals(Analytics.StockTransactions))
                    cbStockItems.Visibility = Visibility.Visible;
            }
        }

        private Analytics GetSelectedAction()
        {
            Tuple<Analytics, string> selected = cbAction.SelectedItem as Tuple<Analytics, string>;
            if (selected != null)
            {
                return selected.Item1;
            }
            return Analytics.None;
        }
    }

    public class ProductItem
    {
        public ObjectId ProductId { get; set; }
        public string Name { get; set; }
    }

    public enum Analytics
    {
        Sales,
        StockTransactions,
        None,
    }
}

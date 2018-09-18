using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeaverCoffeeDemo.ViewModels
{
    public class StockItemViewModel : INotifyPropertyChanged
    {

        private double amount;

        public ObjectId Id { get; set; }
        public ObjectId ProductId { get; set; }
        public string ProductName { get; set; }
        public string Unit { get; set; }
        public double StartingAmount { get; set; }


        public double Amount
        {
            get { return this.amount; }

            set
            {
                if (value != this.amount)
                {
                    StartingAmount = this.amount;
                    this.amount = value;
                    NotifyPropertyChanged("Amount");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
    }
}

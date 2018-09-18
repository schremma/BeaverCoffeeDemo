using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeaverCoffeeDemo.ViewModels
{
    public class AddressViewModel : INotifyPropertyChanged
    {
        private string city;
        private string street;
        private string zip;
        private string country;

        public AddressViewModel() { }

        public ObjectId OwnerId { get; set; }

        public string Street
        {
            get { return this.street; }

            set
            {
                if (value != this.street)
                {
                    this.street = value;
                    NotifyPropertyChanged("Street");
                }
            }
        }

        public string Zip
        {
            get { return this.zip; }

            set
            {
                if (value != this.zip)
                {
                    this.zip = value;
                    NotifyPropertyChanged("Zip");
                }
            }
        }
        public string Country
        {
            get { return this.country; }

            set
            {
                if (value != this.country)
                {
                    this.country = value;
                    NotifyPropertyChanged("Country");
                }
            }
        }

        public string City
        {
            get { return this.city; }

            set
            {
                if (value != this.city)
                {
                    this.city = value;
                    NotifyPropertyChanged("City");
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

        public override string ToString()
        {
            return String.Format("{0} {1}, {2}", Street, City, Country);
        }
    }
}

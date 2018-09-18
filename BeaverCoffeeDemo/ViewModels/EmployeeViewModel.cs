using BeaverCoffeeDemo.Data;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeaverCoffeeDemo.ViewModels
{
    public class EmployeeViewModel : INotifyPropertyChanged
    {
        private string name;
        private string personalId;
        private DateTime hireDate;
        private DateTime employementEndDate;
        private AddressViewModel address;
       

        public EmployeeViewModel() { }

        public Location AssociatedLocation { get; set; }
        public string PersonalId
        {
            get { return this.personalId; }

            set
            {
                if (value != this.personalId)
                {
                    this.personalId = value;
                    NotifyPropertyChanged("PersonalId");
                }
            }
        }
        public DateTime HireDate
        {
            get { return this.hireDate; }

            set
            {
                if (value != this.hireDate)
                {
                    this.hireDate = value;
                    NotifyPropertyChanged("HireDate");
                }
            }
        }
        public DateTime EmployementEndDate
        {
            get { return this.employementEndDate; }

            set
            {
                if (value != this.employementEndDate)
                {
                    this.employementEndDate = value;
                    NotifyPropertyChanged("EmployementEndDate");
                }
            }
        }
        public AddressViewModel Address
        {
            get { return this.address; }

            set
            {
                if (value != this.address)
                {
                    this.address = value;
                    NotifyPropertyChanged("Address");
                }
            }
        }

        public IEnumerable<EmployementPosition> Positions { get; set; }


        public String Name
        {
            get { return this.name; }

            set
            {
                if (value != this.name)
                {
                    this.name = value;
                    NotifyPropertyChanged("Name");
                }
            }
        }
        public ObjectId Id { get; set; }

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

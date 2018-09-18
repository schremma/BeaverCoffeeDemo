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
    public class PositionViewModel : INotifyPropertyChanged
    {

        private int position;
        private string workLocation;
        private ObjectId shopId;
        private DateTime startDate;
        private DateTime endDate;
        private float fullTimeProcent;
        private AddressViewModel shopAddress;

        public ObjectId EmployeeId { get; set; }

        public string WorkLocation
        {
            get { return this.workLocation; }

            set
            {
                if (value != this.workLocation)
                {
                    this.workLocation = value;
                    NotifyPropertyChanged("WorkLocation");
                }
            }
        }


        public ObjectId ShopId
        {
            get { return this.shopId; }

            set
            {
                if (value != this.shopId)
                {
                    this.shopId = value;
                    NotifyPropertyChanged("ShopId");
                }
            }
        }

        public AddressViewModel ShopAddress
        {
            get { return this.shopAddress; }

            set
            {
                if (value != this.shopAddress)
                {
                    this.shopAddress = value;
                    NotifyPropertyChanged("ShopAddress");
                }
            }
        }

        public DateTime StartDate
        {
            get { return this.startDate; }

            set
            {
                if (value != this.startDate)
                {
                    this.startDate = value;
                    NotifyPropertyChanged("StartDate");
                }
            }
        }

        public DateTime EndDate
        {
            get { return this.endDate; }

            set
            {
                if (value != this.endDate)
                {
                    this.endDate = value;
                    NotifyPropertyChanged("EndDate");
                }
            }
        }

        public float FullTimeProcent
        {
            get { return this.fullTimeProcent; }

            set
            {
                if (value != this.position)
                {
                    this.fullTimeProcent = value;
                    NotifyPropertyChanged("FullTimeProcent");
                }
            }
        }



        public int Position
        {
            get { return this.position; }

            set
            {
                if (value != this.position)
                {
                    this.position = value;
                    NotifyPropertyChanged("Position");
                }
            }
        }

        public ObjectId Id { get; set; }
        public IEnumerable<ObjectId> BossOver { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        public override bool Equals(Object obj)
        {
            PositionViewModel pos2 = obj as PositionViewModel;
            if (pos2 != null)
            {
                return (this.EmployeeId == pos2.EmployeeId && this.FullTimeProcent == pos2.FullTimeProcent &&
                    this.Position == pos2.Position && this.ShopId == pos2.ShopId && this.StartDate == pos2.StartDate);
            }
            return false;
        }

        public bool Equals(EmployementPosition obj)
        {
            EmployementPosition pos2 = obj as EmployementPosition;
            if (pos2 != null)
            {
                return (this.FullTimeProcent == pos2.FullTimeProcent &&
                    this.Position == pos2.Position && this.StartDate == pos2.StartDate);
            }
            return false;
        }
    }
}

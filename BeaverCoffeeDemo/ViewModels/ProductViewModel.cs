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
    public class ProductViewModel : OrderItem, INotifyPropertyChanged
    {
        private int unit;

        public String Name { get; set; }
        public List<ProductOptionViewModel> AddOnList { get; set; }
        public ObjectId AddedTo { get; set; }
        public ProductCategory Category { get; set; }
        public IngredientsViewModel Ingredients { get; set; }
        public int OriginalUnit { get; set; }

        public int Unit
        {
            get { return this.unit; }

            set
            {
                if (value != this.unit)
                {
                    OriginalUnit = this.unit;
                    this.unit = value;
                    UnitAmount = value;
                    NotifyPropertyChanged("Unit");
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

    public class IngredientsViewModel
    {
        public IEnumerable<BaseIngredientViewModel> BaseIngredients { get; set; }
        public MilkTypeIngredientsViewModel MilkTypeIngredients { get; set; }
        public IEnumerable<ProductOptionViewModel> ExtraIngredients { get; set; }
    }

    public class BaseIngredientViewModel
    {
        public ObjectId Id { get; set; }
        public int StockUnit { get; set; }
        public String Name { get; set; }
    }

    public class MilkTypeIngredientsViewModel
    {
        public int StockUnit { get; set; }
        public IEnumerable<ProductOptionViewModel> Options { get; set; }
        public ProductOptionViewModel SelectedOption { get; set; }
    }

    public class ProductOptionViewModel
    {
        public ObjectId Id { get; set; }
        public String Name { get; set; }
    }
}

using BeaverCoffeeDemo.Data;
using BeaverCoffeeDemo.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace BeaverCoffeeDemo.ValidationRules
{
    public class PositionValidation : ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            if (value != null)
            {

                PositionViewModel position = (value as BindingGroup).Items[0] as PositionViewModel;

                if (position.StartDate < new DateTime(1900, 01, 01, 00, 00, 00))
                {
                    return new ValidationResult(false, "Enter a valid start date");
                }
                if (!string.IsNullOrEmpty(position.WorkLocation))
                {
                    var loc = DbManager.GetInstance().GetLocationForName(position.WorkLocation);
                    if (loc == null)
                    {
                        return new ValidationResult(false, "Name is not valid for location");
                    }

                }
                return ValidationResult.ValidResult;
            }
            return new ValidationResult(false, "Fill in the row");
        }
    }
}

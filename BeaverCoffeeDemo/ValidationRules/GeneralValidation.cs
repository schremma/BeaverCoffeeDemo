using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace BeaverCoffeeDemo.ValidationRules
{
    public class GeneralValidation : ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            if (value != null)
            {
                if (string.IsNullOrEmpty((value.ToString())))
                    return new ValidationResult(false, "Field cannot be empty");
                return ValidationResult.ValidResult;
            }
            return new ValidationResult(false, "Field cannot be empty");
        }
    }
}

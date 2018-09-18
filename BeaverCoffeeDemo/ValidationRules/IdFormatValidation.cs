using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace BeaverCoffeeDemo.ValidationRules
{
    public class IdFormatValidation : ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            if (value != null)
            {
                String format = ShopGlobals.Location.PersonalIdFormat;
                Regex r = new Regex(format);
                Match m = r.Match(value.ToString());
                if (!m.Success)
                    return new ValidationResult(false, "Correct id format is: " + ValidationHelper.IdRegexToString(format));
                return ValidationResult.ValidResult;
            }
            return new ValidationResult(false, "Field cannot be empty");
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace BeaverCoffeeDemo.ValidationRules
{
    public class CommentValidation : ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            if (value != null)
            {
                if (string.IsNullOrEmpty((value.ToString())))
                    return new ValidationResult(false, "Comment cannot be empty");
                if (value.ToString().Length > 300)
                    return new ValidationResult(false, "Comment cannot be longer than 300 characters");
                return ValidationResult.ValidResult;
            }
            return new ValidationResult(false, "Comment cannot be empty");
        }
    }
}

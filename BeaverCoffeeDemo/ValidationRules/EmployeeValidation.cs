
using BeaverCoffeeDemo.Data;
using BeaverCoffeeDemo.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace BeaverCoffeeDemo.ValidationRules
{
    public class EmployeeValidation : ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            if (value != null)
            {

                EmployeeViewModel employee = (value as BindingGroup).Items[0] as EmployeeViewModel;

                if (!string.IsNullOrEmpty(employee.PersonalId) && employee.AssociatedLocation != null)
                {
                    String format = employee.AssociatedLocation.PersonalIdFormat;
                    Regex r = new Regex(format);
                    Match m = r.Match(employee.PersonalId);
                    if (!m.Success)
                        return new ValidationResult(false, "Correct id format is: " + ValidationHelper.IdRegexToString(format));
                }
                else if (string.IsNullOrEmpty(employee.PersonalId))
                {
                    return new ValidationResult(false, "Personal id cannot be empty");
                }
                else if (employee.AssociatedLocation == null || string.IsNullOrEmpty(employee.AssociatedLocation.PersonalIdFormat))
                {
                    return new ValidationResult(false, "Personal id cannot be edited under 'All' locations");
                }
                if (employee.Address == null)
                        return new ValidationResult(false, "Add address");
                if (string.IsNullOrEmpty(employee.Address.City) || string.IsNullOrEmpty(employee.Address.Street) || string.IsNullOrEmpty(employee.Address.Zip))
                {
                    return new ValidationResult(false, "Fill in address fields");
                }
                if (employee.HireDate < new DateTime(1900, 01, 01, 00, 00, 00))
                {
                    return new ValidationResult(false, "Enter a valid hire date");
                }
                if (string.IsNullOrEmpty(employee.Name))
                {
                    return new ValidationResult(false, "Enter employee's name");

                }
                return ValidationResult.ValidResult;

            }
            return new ValidationResult(false, "Fill in the row");
        }
    }
}

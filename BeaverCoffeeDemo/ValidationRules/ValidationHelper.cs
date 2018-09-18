using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BeaverCoffeeDemo.ValidationRules
{
    public class ValidationHelper
    {
        public static string IdRegexToString(String regex)
        {
            switch (regex)
            {
                case @"(?<!\d)\d{10}(?!\d)": return "10 digits";
                case "^[A-Za-z]{6}[0-9]{2}$": return "six letters followed by two digits";
                default: return string.Empty;
            }
        }

        public static bool ValidateIdFormat(string format, string idString)
        {
            Regex r = new Regex(format);
            Match m = r.Match(idString);
            return m.Success;
        }
    }
}

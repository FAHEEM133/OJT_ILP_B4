using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validations
{
    public static class SubGroupValidation
    {
        // Method to validate if a string contains only alphabets and a single space
        public static bool IsAlphabetic(string subGroupName)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(subGroupName, @"^[a-zA-Z]+( [a-zA-Z]+)*$");
        }
    }
}

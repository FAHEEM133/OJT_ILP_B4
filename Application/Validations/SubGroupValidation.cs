using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Application.Validations
{
    public static class SubGroupValidation
    {
        public static bool IsValidSubGroupCode(string subGroupCode)
        {
            return Regex.IsMatch(subGroupCode, @"^[A-Za-z0-9]{1}$");
        }
    }
}

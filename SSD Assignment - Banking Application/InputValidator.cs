using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SSD_Assignment___Banking_Application
{
    internal class InputValidator
    {
        public bool IsValidString(string input)
        {
            // Regular expression pattern allowing only alphanumeric characters and spaces
            string pattern = @"^[a-zA-Z0-9\s]+$";

            // Check if the input matches the pattern
            return Regex.IsMatch(input, pattern);
        }

        public bool IsValidDouble(string input, out double balance)
        {
            if (double.TryParse(input, out balance) && balance > 0)
            {
                return true;
            }

            return false;
        }
    }
}

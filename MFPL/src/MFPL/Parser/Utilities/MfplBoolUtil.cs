using MFPL.Functional;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MFPL.Parser.Utilities
{
    public static class MfplBoolUtil
    {
        public static Result<bool> Parse(string input)
        {
            bool o;
            if (bool.TryParse(input, out o))
            {
                return Result.Ok(o);
            }
            else
            {
                return Result.Fail<bool>($"Literial '{input}' is not valid bool format.");
            }
        }
    }
}

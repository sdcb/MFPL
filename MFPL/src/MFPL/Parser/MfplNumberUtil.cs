using MFPL.Functional;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MFPL.Parser
{
    public static class MfplNumberUtil
    {
        public static Result<double> Parse(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return Result.Fail<double>($"number literial must not be empty.");

            double n;
            if (double.TryParse(input, out n))
            {
                return Result.Ok(n);
            }
            else
            {
                return Result.Fail<double>("number literial format error.");
            }
        }
    }
}

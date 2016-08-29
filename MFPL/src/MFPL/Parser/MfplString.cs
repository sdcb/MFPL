using MFPL.Functional;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MFPL.Parser.Utilities
{
    public static class MfplString
    {
		public static Result<string> Escape(string text)
		{
			if (string.IsNullOrWhiteSpace(text))
				return Result.Fail<string>($"string literial must not be empty.");

			if (text.Length <= 2)
				return Result.Fail<string>($"string literial length must > 2.");

			if (text.First() != text.Last())
				return Result.Fail<string>($"string literial's first and last char must be the same.");

			var firstChar = text[0];
			var escapedChar = Details.GetEscapedChar(firstChar);
			if (escapedChar.IsFailure)
				return Result.Fail<string>(escapedChar.Error);

			var inner = text.Substring(1, text.Length - 2);
			return Result.Ok(Details.EscapeNoCheck(inner, escapedChar.Value));
		}

		public class Details
		{
			public static Result<char> GetEscapedChar(char ch)
			{
				if (ch == '\'')
				{
					return Result.Ok('"');
				}
				else if (ch == '"')
				{
					return Result.Ok('\'');
				}
				else
				{
					return Result.Fail<char>($"char {ch} not escappable.");
				}
			}

			public static string EscapeNoCheck(string input, char escapeChar)
			{
                return input
                    .Replace(@"\\", @"\")
                    .Replace(@"\/", @"/")
                    .Replace(@"\t", "\t")
                    .Replace(@"\b", "\b")
                    .Replace(@"\f", "\f")
                    .Replace(@"\n", "\n")
                    .Replace(@"\r", "\r")
                    .Replace(@"\t", "\t")
                    .Replace($@"\{escapeChar}", escapeChar.ToString());
			}
		}
    }
}

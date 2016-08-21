using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using static MFPL.Parser.Utilities.MfplString;

namespace MFPL.Test.Parser
{
    public class MfplStringTest
    {
		[Theory]
		[InlineData('\'', '"')]
		[InlineData('"', '\'')]
		public void GetEscapedCharTest(char input, char expected)
		{
			var ch = Details.GetEscapedChar(input);
			Assert.Equal(expected, ch.Value);
		}
	}
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using static MFPL.Parser.Utilities.MfplStringUtil;

namespace MFPL.Test.Parser
{
    public class MfplStringUtilTest
    {
		[Theory]
		[InlineData('\'', '"')]
		[InlineData('"', '\'')]
		public void GetEscapedCharTest(char input, char expected)
		{
			var ch = Details.GetEscapedChar(input);
			Assert.Equal(expected, ch.Value);
		}

		[Fact]
		public void GetEscapedCharFailedTest()
		{
			var ch = Details.GetEscapedChar('F');
			Assert.True(ch.IsFailure);
		}

        [Fact]
        public void NoEscapeTest()
        {
            var expected = "Hello World";
            var input = $"\"{expected}\"";
            var result = Parse(input);
            Assert.True(result.IsSuccess);
            Assert.Equal(expected, result.Value);
        }

        [Fact]
        public void EscapeChineseTest()
        {
            var expected = "Hello 中文";
            var input = $"'{expected}'";
            var result = Parse(input);
            Assert.True(result.IsSuccess);
            Assert.Equal(expected, result.Value);
        }

        [Theory]
        [InlineData(@"'\t'", "\t")]
        [InlineData(@"'\\'", @"\")]
        [InlineData(@"'\n'", "\n")]
        [InlineData(@"'\r'", "\r")]
        public void EscapeTest(string input, string expected)
        {
            var result = Parse(input);
            Assert.True(result.IsSuccess);
            Assert.Equal(expected, result.Value);
        }

        [Theory]
        [InlineData(@"'Hello \u6211\u662F'", "Hello 我是")]
        [InlineData(@"'Hello \uD852\uDF62'", "Hello 𤭢")]
        public void UnicodeTest(string unicode, string expected)
        {
            var actual = Parse(unicode);
            Assert.Equal(expected, actual.Value);
        }
    }
}

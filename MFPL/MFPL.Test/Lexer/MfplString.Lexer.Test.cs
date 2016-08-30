using MFPL.Compiler;
using MFPL.Parser.G4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace MFPL.Test.Lexer
{
    public class MfplStringLexerTest
    {
        [Theory]
        [InlineData('\'')]
        [InlineData('"')]
        public void EmptyStringTest(char border)
        {
            var source = $"{border}{border}";
            var actual = LexerParseSingle(source);
            Assert.Equal(source, actual);
        }

        [Theory]
        [InlineData('\'', '"')]
        [InlineData('"', '\'')]
        public void NotEscapeCharOk(char escape, char border)
        {
            var source = $"{border}{escape}{border}";
            var actual = LexerParseSingle(source);
            Assert.Equal(source, actual);
        }

        [Theory]
        [InlineData('\'')]
        [InlineData('"')]
        public void EscapeCharOk(char escape)
        {
            var source = $@"{escape}\{escape}{escape}";
            var actual = LexerParseSingle(source);
            Assert.Equal(source, actual);
        }

        [Fact]
        public void SupportChinese()
        {
            var source = "'Hello 中文'";
            var actual = LexerParseSingle(source);
            Assert.Equal(source, actual);
        }

        [Fact]
        public void SupportUnicode()
        {
            var source = "\"\\u6211\"";
            var actual = LexerParseSingle(source);
            Assert.Equal(source, actual);
        }

        static string LexerParseSingle(string sourceCode)
        {
            var lexer = MfplCompiler.Helper.BuildMfplLexer(sourceCode);

            var tokens = lexer.GetAllTokens();
            Assert.Equal(1, tokens.Count);

            var token = tokens[0];
            Assert.Equal(MfplLexer.STRING, token.Type);

            return token.Text;
        }
    }
}

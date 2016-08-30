using MFPL.Compiler;
using MFPL.Parser.G4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace MFPL.Test.Parser
{
    public class MfplSyntaxTest
    {
        [Theory]
        [InlineData("test")]
        [InlineData("F123")]
        [InlineData("_")]
        [InlineData("_3")]
        public void ValidSyntaxTest(string sourceCode)
        {
            var lexer = MfplCompiler.Helper.BuildMfplLexer(sourceCode);
            var tokens = lexer.GetAllTokens();

            Assert.Equal(1, tokens.Count);
            var token = tokens[0];
            Assert.Equal(MfplLexer.SYNTAX, token.Type);
        }

        [Theory]
        [InlineData("0")]
        [InlineData("3F")]
        public void InvalidSyntaxTest(string sourceCode)
        {
            var lexer = MfplCompiler.Helper.BuildMfplLexer(sourceCode);
            var tokens = lexer.GetAllTokens();


        }
    }
}

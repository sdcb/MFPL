using Antlr4.Runtime;
using MFPL.Compiler;
using MFPL.Parser.G4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace MFPL.Test.Lexer
{
    public class MfplTokenTest
    {
        [Fact]
        public void TokenCanSplit()
        {
            var input = "3 '3'";
            var lexer = MfplCompiler.Helper.BuildMfplLexer(input);

            var tokens = lexer.GetAllTokens();
            Assert.Equal(2, tokens.Count);

            Assert.Equal(MfplLexer.NUMBER, tokens[0].Type);
            Assert.Equal(MfplLexer.STRING, tokens[1].Type);
        }

        [Fact]
        public void CannotParseUnknownToken()
        {
            var input = "3 \"3";

            var lexer = MfplCompiler.Helper.BuildMfplLexer(input);
            var errorListener = new ErrorListener();
            lexer.AddErrorListener(errorListener);

            var tokens = lexer.GetAllTokens();
            
            Assert.Equal(1, errorListener.Errors.Count);
            Assert.Equal(1, tokens.Count);
        }

        private class ErrorListener : IAntlrErrorListener<int>
        {
            public void SyntaxError(IRecognizer recognizer, int offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
            {
                this.Errors.Add(msg);
            }

            public List<string> Errors { get; set; } = new List<string>();
        }
    }
}

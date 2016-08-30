using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using MFPL.Parser.G4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MFPL.Compiler
{
    public class MfplCompiler
    {
        public class Helper
        {
            public static MfplParser BuildMfplParser(string sourceCode)
            {
                var inputStream = new AntlrInputStream(sourceCode);
                var lexer = new MfplLexer(inputStream);
                var tokenStream = new CommonTokenStream(lexer);
                var parser = new MfplParser(tokenStream);
                return parser;
            }

            public static MfplLexer BuildMfplLexer(string sourceCode)
            {
                var inputStream = new AntlrInputStream(sourceCode);
                var lexer = new MfplLexer(inputStream);
                return lexer;
            }
        }
    }
}

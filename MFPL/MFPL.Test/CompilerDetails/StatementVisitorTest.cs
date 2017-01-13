using MFPL.Compiler;
using MFPL.Compiler.Core;
using MFPL.Compiler.Visitors;
using Sigil;
using Sigil.NonGeneric;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading.Tasks;
using Xunit;

namespace MFPL.Test.CompilerDetails
{
    public class StatementVisitorTest
    {
        [Fact]
        public void ValueExpressionCannotBeStatement()
        {
            var sourceCode = "true;";
            var parser = MfplCompiler.Helper.BuildMfplParser(sourceCode);
            var visitor = new StatementVisitor(GetILBuilder(), ContextScope.CreateEmpty<Local>());
            var result = visitor.Visit(parser.root());

            Assert.True(result.IsFailure);
        }

        [Fact]
        public void FunctionCallCanBeStatement()
        {
            var sourceCode = "printHelloWorld();";
            var parser = MfplCompiler.Helper.BuildMfplParser(sourceCode);
            var visitor = new StatementVisitor(GetILBuilder(), ContextScope.CreateEmpty<Local>());
            var result = visitor.Visit(parser.root());

            Assert.True(result.IsSuccess);
        }

        [Fact]
        public void BinaryExpressionCannotBeStatement()
        {
            var sourceCode = "3+4;";
            var parser = MfplCompiler.Helper.BuildMfplParser(sourceCode);
            var visitor = new StatementVisitor(GetILBuilder(), ContextScope.CreateEmpty<Local>());
            var result = visitor.Visit(parser.root());

            Assert.True(result.IsFailure);
        }

        private Emit GetILBuilder()
        {
            return Emit.NewDynamicMethod(typeof(void).GetType(), Type.EmptyTypes);
        }
    }
}

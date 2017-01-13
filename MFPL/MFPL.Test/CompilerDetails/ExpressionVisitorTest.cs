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
    public class ExpressionVisitorTest
    {
        [Fact]
        public void Smoke()
        {
            var il = MakeEmptyFunction();
            var sourceCode = "true";
            var parser = MfplCompiler.Helper.BuildMfplParser(sourceCode);
            var visitor = new ExpressionVisitor(il, ContextScope.CreateEmpty<Local>());
            var result = visitor.Visit(parser.root());

            Assert.True(result.IsSuccess);
            var instruction = il.Instructions();
            Assert.Equal("ldc.i4.1", instruction);
        }

        [Fact]
        public void GetNonExistingVarShouldFail()
        {
            var sourceCode = "a+b";
            var parser = MfplCompiler.Helper.BuildMfplParser(sourceCode);
            var visitor = new ExpressionVisitor(MakeEmptyFunction(), ContextScope.CreateEmpty<Local>());
            var result = visitor.Visit(parser.root());

            Assert.True(result.IsFailure);
        }

        [Fact]
        public void CanGetExistingVar()
        {
            var il = MakeEmptyFunction();
            var scope = ContextScope.CreateEmpty<Local>();
            scope.Declare("a", il.DeclareLocal(typeof(double)));
            scope.Declare("b", il.DeclareLocal(typeof(double)));

            var sourceCode = "a+b";
            var parser = MfplCompiler.Helper.BuildMfplParser(sourceCode);
            var visitor = new ExpressionVisitor(il, scope);
            var result = visitor.Visit(parser.root());

            Assert.True(result.IsSuccess);
            var insr = il.Instructions();
            Assert.Equal(
                "ldloc.0 // System.Double _local0\r\n" +
                "ldloc.1 // System.Double _local1\r\n" +
                "add", insr);
        }

        private Emit MakeEmptyFunction()
        {
            return Emit.NewDynamicMethod(typeof(void).GetType(), Type.EmptyTypes);
        }
    }
}

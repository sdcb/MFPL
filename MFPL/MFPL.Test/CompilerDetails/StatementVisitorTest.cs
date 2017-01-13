using MFPL.Compiler;
using MFPL.Compiler.Core;
using MFPL.Compiler.Visitors;
using MFPL.Functional;
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
            var il = CreateEmiter();
            var result = Run(il, "true");

            Assert.True(result.IsFailure);
        }

        [Fact]
        public void CallReturnWillEmitPop()
        {
            var il = CreateEmiter();
            var result = Run(il, "abs(3);");

            var instructions = il.Instructions();
            Assert.Equal(
                "ldc.r8 3\r\n" +
                "call Double Abs(Double)\r\n" + 
                "pop", instructions);
        }

        [Fact]
        public void CallNoReturnWontEmitPop()
        {
            var il = CreateEmiter();
            var result = Run(il, "print(3);");

            var instructions = il.Instructions();
            Assert.Equal(
                "ldc.r8 3\r\n" + 
                "call Void WriteLine(Double)", instructions);
        }

        [Fact]
        public void BinaryExpressionCannotBeStatement()
        {
            var il = CreateEmiter();
            var sourceCode = "3+4;";
            var result = Run(il, sourceCode);

            Assert.True(result.IsFailure);
        }

        private Emit CreateEmiter()
        {
            return Emit.NewDynamicMethod(typeof(void).GetType(), Type.EmptyTypes);
        }

        private Result Run(Emit il, string sourceCode)
        {
            var parser = MfplCompiler.Helper.BuildMfplParser(sourceCode);
            var visitor = new StatementVisitor(il, ContextScope.CreateEmpty<Local>());
            return visitor.Visit(parser.root());
        }
    }
}

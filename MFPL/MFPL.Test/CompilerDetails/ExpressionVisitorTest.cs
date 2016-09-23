using MFPL.Compiler;
using MFPL.Compiler.Core;
using MFPL.Compiler.Visitors;
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
            var sourceCode = "true;";
            var parser = MfplCompiler.Helper.BuildMfplParser(sourceCode);
            var visitor = new ExpressionVisitor(ContextScope.CreateEmpty<LocalBuilder>());
            var result = visitor.Visit(parser.root());

            Assert.True(result.IsSuccess);
            var expected = new List<Instruction>
            {
                Instruction.Create(OpCodes.Ldc_I4_1)
            };
            Assert.Equal(expected, result.Value.Instructions);
        }
    }
}

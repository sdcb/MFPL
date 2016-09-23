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
            var sourceCode = "true";
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

        [Fact]
        public void GetNonExistingVarShouldFail()
        {
            var sourceCode = "a+b";
            var parser = MfplCompiler.Helper.BuildMfplParser(sourceCode);
            var visitor = new ExpressionVisitor(ContextScope.CreateEmpty<LocalBuilder>());
            var result = visitor.Visit(parser.root());

            Assert.True(result.IsFailure);
        }

        [Fact]
        public void CanGetExistingVar()
        {
            var il = GetILBuilder();
            var scope = ContextScope.CreateEmpty<LocalBuilder>();
            scope.Declare("a", il.DeclareLocal(typeof(double)));
            scope.Declare("b", il.DeclareLocal(typeof(double)));

            var sourceCode = "a+b";
            var parser = MfplCompiler.Helper.BuildMfplParser(sourceCode);
            var visitor = new ExpressionVisitor(scope);
            var result = visitor.Visit(parser.root());

            Assert.True(result.IsSuccess);
        }

        private ILGenerator GetILBuilder()
        {
            var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(
                new AssemblyName(Guid.NewGuid().ToString()), AssemblyBuilderAccess.RunAndCollect);
            var module = assemblyBuilder
                .DefineDynamicModule(Guid.NewGuid().ToString());
            var method = module.DefineGlobalMethod(Guid.NewGuid().ToString(),
                MethodAttributes.Public | MethodAttributes.Static, typeof(void), Type.EmptyTypes);
            return method.GetILGenerator();
        }
    }
}

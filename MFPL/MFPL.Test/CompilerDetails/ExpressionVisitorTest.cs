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
            var visitor = new ExpressionVisitor(GetILBuilder(), ContextScope.CreateEmpty<LocalBuilder>());
            var result = visitor.Visit(parser.root());
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

using MFPL.Functional;
using MFPL.Parser.G4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Antlr4.Runtime.Misc;
using static MFPL.Parser.G4.MfplParser;
using System.Reflection.Emit;

namespace MFPL.Compiler.Details
{
    public class StatementVisitor : MfplBaseVisitor<Result>
    {
        private readonly ILGenerator il;
        private readonly ContextScope<LocalBuilder> scope;

        public StatementVisitor(ILGenerator il, ContextScope<LocalBuilder> scope)
        {
            this.il = il;
            this.scope = scope;
        }

        public override Result VisitExpressionStatement([NotNull] ExpressionStatementContext context)
        {
            var expression = context.GetChild<ExpressionContext>(0);
            if (expression is FunctionCallExpressionContext)
            {
                return Result.Ok();
            }
            else
            {
                return Result.Fail("Statement expression can only be function call.");
            }
        }
    }
}

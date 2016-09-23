using MFPL.Functional;
using MFPL.Parser.G4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Antlr4.Runtime.Misc;
using static MFPL.Parser.G4.MfplParser;
using System.Reflection.Emit;
using MFPL.Compiler.Core;

namespace MFPL.Compiler.Visitors
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

            return Result.Ok(expression)
                .Ensure(v => v is FunctionCallExpressionContext, "Statement expression can only be function call.")
                .OnSuccess(v => VisitExpression(v))
                .ExecWhen(t => t != MfplTypes.Void, t => il.Emit(OpCodes.Pop));
        }

        public new Result<MfplTypes> VisitExpression([NotNull] ExpressionContext context)
        {
            var v = new ExpressionVisitor(scope).Visit(context);
            return v
                .OnSuccess(ei => ei.EmitAll(il))
                .OnSuccess(() => v.Value.ResultType);
        }
    }
}

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
using Sigil.NonGeneric;
using Sigil;

namespace MFPL.Compiler.Visitors
{
    public class StatementVisitor : MfplBaseVisitor<Result>
    {
        private readonly Emit il;
        private readonly ContextScope<Local> scope;

        public StatementVisitor(Emit il, ContextScope<Local> scope)
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
                .ExecWhen(t => t != MfplTypes.Void, t => il.Pop());
        }

        public new Result<MfplTypes> VisitExpression([NotNull] ExpressionContext context)
        {
            return new ExpressionVisitor(il, scope).Visit(context);
        }
    }
}

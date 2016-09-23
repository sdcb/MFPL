using MFPL.Functional;
using MFPL.Parser.G4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Threading.Tasks;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using static MFPL.Parser.G4.MfplParser;
using MFPL.Parser.Utilities;
using MFPL.Parser;
using System.Reflection;
using MFPL.Compiler.MfplLibs;
using MFPL.Compiler.Core;

namespace MFPL.Compiler.Visitors
{
    public class ExpressionVisitor : MfplBaseVisitor<Result<ExpressionInstruction>>
    {
        private readonly ContextScope<LocalBuilder> scope;

        public ExpressionVisitor(ContextScope<LocalBuilder> scope)
        {
            this.scope = scope;
        }

        public override Result<ExpressionInstruction> VisitValueExpression([NotNull] ValueExpressionContext context)
        {
            var node = context.GetChild<ValueContext>(0).GetChild<TerminalNodeImpl>(0).Symbol;
            switch (node.Type)
            {
                case MfplLexer.STRING:
                    return MfplStringUtil.Parse(node.Text).OnSuccess(str =>
                    {
                        return ExpressionInstruction.FromValue(str);
                    });
                case MfplLexer.NUMBER:
                    return MfplNumberUtil.Parse(node.Text).OnSuccess(num =>
                    {
                        return ExpressionInstruction.FromValue(num);
                    });
                case MfplLexer.BOOL:
                    return MfplBoolUtil.Parse(node.Text).OnSuccess(b =>
                    {
                        return ExpressionInstruction.FromValue(b);
                    });
                default:
                    return Result.Fail<ExpressionInstruction>($"Unkown type for literial '{context.GetText()}'.");
            }
        }

        public override Result<ExpressionInstruction> VisitUnaryExpression([NotNull] UnaryExpressionContext context)
        {
            var exp1 = Visit(context.GetChild<ExpressionContext>(0));
            var op = context.GetChild(0).GetText();

            return exp1.OnSuccess(ei => ei.ByUnaryOperation(op));
        }

        public override Result<ExpressionInstruction> VisitBinaryExpression([NotNull] BinaryExpressionContext context)
        {
            var exp1 = Visit(context.GetChild<ExpressionContext>(0));
            var exp2 = Visit(context.GetChild<ExpressionContext>(1));
            var op = context.GetChild(1).GetText();

            return exp1
                .OnSuccess(_ => exp2)
                .OnSuccess(_ => exp1.Value.ByBinaryOperator(op, exp2.Value));
        }

        public override Result<ExpressionInstruction> VisitFunctionCallExpression([NotNull] FunctionCallExpressionContext context)
        {
            var syntax = context.GetChild(0).GetText();
            var expressions = context.children.OfType<ExpressionContext>();

            var expTypes = expressions.Select(x => Visit(x)).ToList();
            foreach (var type in expTypes)
            {
                if (type.IsFailure) return type;
            }

            var types = expTypes.Select(x => x.Value.ResultType).ToArray();
            var function = KnownFunction.GetFunction(syntax, types);
            return Result.Ok(function)
                .OnSuccess(v =>
                {
                    return ExpressionInstruction.CombineWithType(
                        v.Value.ResultType,
                        expTypes.Select(x => x.Value));
                });
        }

        protected override Result<ExpressionInstruction> AggregateResult(
            Result<ExpressionInstruction> aggregate, 
            Result<ExpressionInstruction> nextResult)
        {
            if (aggregate != null && nextResult == null)
            {
                return aggregate;
            }
            else if (aggregate == null && nextResult != null)
            {
                return nextResult;
            }
            else
            {
                throw new ArgumentException("Expression should never eval to multiple result.");
            }
        }
    }
}

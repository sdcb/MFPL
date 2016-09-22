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
        private readonly ILGenerator il;
        private readonly ContextScope<LocalBuilder> scope;

        public ExpressionVisitor(ILGenerator il, ContextScope<LocalBuilder> scope)
        {
            this.il = il;
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
                        return ExpressionInstruction.Create(
                            Instruction.Create(OpCodes.Ldstr, str), 
                            MfplTypes.String);
                    });
                case MfplLexer.NUMBER:
                    return MfplNumberUtil.Parse(node.Text).OnSuccess(num =>
                    {
                        return ExpressionInstruction.Create(
                            Instruction.Create(OpCodes.Ldc_R8, num),
                            MfplTypes.Number);
                    });
                case MfplLexer.BOOL:
                    return MfplBoolUtil.Parse(node.Text).OnSuccess(b =>
                    {
                        return ExpressionInstruction.Create(
                            Instruction.Create(b ? OpCodes.Ldc_I4_1 : OpCodes.Ldc_I4_0),
                            MfplTypes.Bool);
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
            throw new NotImplementedException();
            //var syntax = context.GetChild(0).GetText();
            //var expressions = context.children.OfType<ExpressionContext>();

            //var expTypes = expressions.Select(x => Visit(x)).ToList();
            //foreach (var type in expTypes)
            //{
            //    if (type.IsFailure) return type;
            //}

            //var types = expTypes.Select(x => x.Value).ToArray();

            //var function = KnownFunction.GetFunction(syntax, types);
            //if (function.IsFailure) return Result.Fail<MfplTypes>(function.Error);

            //function.Value.Emiter(il);
            //return Result.Ok(function.Value.ReturnType);
        }
    }
}

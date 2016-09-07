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

namespace MFPL.Compiler.Details
{
    public class ExpressionVisitor : MfplBaseVisitor<Result<MfplTypes>>
    {
        private readonly ILGenerator il;
        private readonly ContextScope<LocalBuilder> scope;

        public ExpressionVisitor(ILGenerator il, ContextScope<LocalBuilder> scope)
        {
            this.il = il;
            this.scope = scope;
        }

        public override Result<MfplTypes> VisitValueExpression([NotNull] ValueExpressionContext context)
        {
            var node = context.GetChild<ValueContext>(0).GetChild<TerminalNodeImpl>(0).Symbol;
            switch (node.Type)
            {
                case MfplLexer.STRING:
                    return MfplStringUtil.Parse(node.Text).OnSuccess(str =>
                    {
                        il.Emit(OpCodes.Ldstr, str);
                        return MfplTypes.String;
                    });
                case MfplLexer.NUMBER:
                    return MfplNumberUtil.Parse(node.Text).OnSuccess(num =>
                    {
                        il.Emit(OpCodes.Ldc_R8, num);
                        return MfplTypes.Number;
                    });
                case MfplLexer.BOOL:
                    return MfplBoolUtil.Parse(node.Text).OnSuccess(b =>
                    {
                        il.Emit(b ? OpCodes.Ldc_I4_1 : OpCodes.Ldc_I4_0);
                        return MfplTypes.Bool;
                    });
                default:
                    return Result.Fail<MfplTypes>($"Unkown type for literial '{context.GetText()}'.");
            }
        }

        public override Result<MfplTypes> VisitUnaryExpression([NotNull] UnaryExpressionContext context)
        {
            var exp1 = Visit(context.GetChild<ExpressionContext>(0));
            var op = context.GetChild(0).GetText();

            return MfplTypeUtil.UnaryOperator(op, exp1).OnSuccess(type => 
            {
                switch (op)
                {
                    case "-":
                        il.Emit(OpCodes.Neg);
                        break;
                    case "!":
                        il.Emit(OpCodes.Not);
                        break;
                    default:
                        return Result.Fail<MfplTypes>($"Unknown unary operator: '{op}'.");
                }
                return Result.Ok(type);
            });
        }

        public override Result<MfplTypes> VisitBinaryExpression([NotNull] BinaryExpressionContext context)
        {
            var exp1 = Visit(context.GetChild<ExpressionContext>(0));
            var exp2 = Visit(context.GetChild<ExpressionContext>(1));
            var op = context.GetChild(1).GetText();

            return MfplTypeUtil.BinaryOperator(exp1, exp2, op).OnSuccess(type => 
            {
                switch (op)
                {
                    case "*":
                        il.Emit(OpCodes.Mul);
                        break;
                    case "/":
                        il.Emit(OpCodes.Div);
                        break;
                    case "+":
                        if (type == MfplTypes.Number)
                        {
                            il.Emit(OpCodes.Add);
                        }
                        else if (type == MfplTypes.String)
                        {
                            il.Emit(OpCodes.Call, typeof(string).GetMethod(
                                nameof(string.Concat), new[] { typeof(string), typeof(string) }));
                        }
                        break;
                    case "-":
                        il.Emit(OpCodes.Sub);
                        break;
                    case ">":
                        il.Emit(OpCodes.Cgt);
                        break;
                    case "<":
                        il.Emit(OpCodes.Clt);
                        break;
                    case ">=":
                        il.Emit(OpCodes.Clt);
                        il.Emit(OpCodes.Not);
                        break;
                    case "<=":
                        il.Emit(OpCodes.Cgt);
                        il.Emit(OpCodes.Not);
                        break;
                    case "&&":
                        il.Emit(OpCodes.And);
                        break;
                    case "||":
                        il.Emit(OpCodes.Or);
                        break;
                    case "==":
                        il.Emit(OpCodes.Ceq);
                        break;
                    case "!=":
                        il.Emit(OpCodes.Ceq);
                        il.Emit(OpCodes.Not);
                        break;
                    default:
                        return Result.Fail<MfplTypes>($"Unknown binary operator '{op}'.");
                }
                return Result.Ok(type);
            });
        }
    }
}

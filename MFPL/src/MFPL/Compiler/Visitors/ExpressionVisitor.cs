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
using MFPL.Compiler.Core;
using Sigil;
using Sigil.NonGeneric;
using MFPL.KnownFunctions;

namespace MFPL.Compiler.Visitors
{
    public class ExpressionVisitor : MfplBaseVisitor<Result<MfplTypes>>
    {
        private readonly Emit il;
        private readonly ContextScope<Local> scope;

        public ExpressionVisitor(Emit il, ContextScope<Local> scope)
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
                        il.LoadConstant(str);
                        return Result.Ok(MfplTypes.String);
                    });
                case MfplLexer.NUMBER:
                    return MfplNumberUtil.Parse(node.Text).OnSuccess(num =>
                    {
                        il.LoadConstant(num);
                        return Result.Ok(MfplTypes.Number);
                    });
                case MfplLexer.BOOL:
                    return MfplBoolUtil.Parse(node.Text).OnSuccess(b =>
                    {
                        il.LoadConstant(b);
                        return Result.Ok(MfplTypes.Bool);
                    });
                default:
                    return Result.Fail<MfplTypes>($"Unkown type for literial '{context.GetText()}'.");
            }
        }

        public override Result<MfplTypes> VisitUnaryExpression([NotNull] UnaryExpressionContext context)
        {
            var exp1 = Visit(context.GetChild<ExpressionContext>(0));
            var op = context.GetChild(0).GetText();

            return exp1
                .OnSuccess(_ => MfplTypeUtil.UnaryOperator(op, exp1.Value))
                .OnSuccess(type =>
                {
                    switch (op)
                    {
                        case "-":
                            il.Negate();
                            return Result.Ok(type);
                        case "!":
                            il.LoadConstant(0);
                            il.CompareEqual();
                            return Result.Ok(type);
                        default:
                            return Result.Fail<MfplTypes>($"Unknown unary operator: '{op}'.");
                    }
                });
        }

        public override Result<MfplTypes> VisitBinaryExpression([NotNull] BinaryExpressionContext context)
        {
            var exp1 = Visit(context.GetChild<ExpressionContext>(0));
            var exp2 = Visit(context.GetChild<ExpressionContext>(1));
            var op = context.GetChild(1).GetText();

            return exp1
                .OnSuccess(_ => exp2)
                .OnSuccess(_ => MfplTypeUtil.BinaryOperator(exp1.Value, exp2.Value, op))
                .OnSuccess(type =>
                {
                    switch (op)
                    {
                        case "*":
                            il.Multiply();
                            return Result.Ok(type);
                        case "/":
                            il.Divide();
                            return Result.Ok(type);
                        case "+":
                            if (type == MfplTypes.Number)
                            {
                                il.Add();
                                return Result.Ok(type);
                            }
                            else
                            {
                                var method = typeof(string).GetMethod(
                                    nameof(string.Concat), new[] { typeof(string), typeof(string) });
                                il.Call(method);
                                return Result.Ok(type);
                            }
                        case "-":
                            il.Subtract();
                            return Result.Ok(type);
                        case ">":
                            il.CompareGreaterThan();
                            return Result.Ok(type);
                        case "<":
                            il.CompareLessThan();
                            return Result.Ok(type);
                        case ">=":
                            il.CompareLessThan();
                            il.LoadConstant(0);
                            il.CompareEqual();
                            return Result.Ok(type);
                        case "<=":
                            il.CompareGreaterThan();
                            il.LoadConstant(0);
                            il.CompareEqual();
                            return Result.Ok(type);
                        case "&&":
                            il.And();
                            return Result.Ok(type);
                        case "||":
                            il.Or();
                            return Result.Ok(type);
                        case "==":
                            if (type == MfplTypes.String)
                            {
                                var method = typeof(string).GetMethod(
                                    nameof(string.Compare), new[] { typeof(string), typeof(string) });
                                il.Call(method);
                            }
                            else
                            {
                                il.CompareEqual();
                            }
                            return Result.Ok(type);
                        case "!=":
                            if (type == MfplTypes.String)
                            {
                                var method = typeof(string).GetMethod(
                                    nameof(string.Compare), new[] { typeof(string), typeof(string) });
                                il.Call(method);
                                il.LoadConstant(0);
                                il.CompareEqual();
                            }
                            else
                            {
                                il.CompareEqual();
                                il.LoadConstant(0);
                                il.CompareEqual();
                            }
                            return Result.Ok(type);
                        default:
                            return Result.Fail<MfplTypes>($"Unknown binary operator '{op}'.");
                    }
                });
        }

        public override Result<MfplTypes> VisitFunctionCallExpression([NotNull] FunctionCallExpressionContext context)
        {
            var syntax = context.GetChild(0).GetText();
            var expressions = context.children.OfType<ExpressionContext>();

            var expTypes = expressions.Select(x => Visit(x)).ToList();
            foreach (var type in expTypes)
            {
                if (type.IsFailure) return type;
            }

            return KnownFunction.Get(syntax, expTypes.Select(x => x.Value).ToList())
                .OnSuccess(m =>
                {
                    il.Call(m);
                    return MfplTypeUtil.TypeToMfplType(m.ReturnType);
                });
        }

        public override Result<MfplTypes> VisitSyntaxExpression([NotNull] SyntaxExpressionContext context)
        {
            return Result.Ok(context.GetChild(0).GetText())
                .OnSuccess(syntax => scope.Get(syntax))
                .OnSuccess(local =>
                {
                    il.LoadLocal(local);
                    return MfplTypeUtil.TypeToMfplType(local.LocalType);
                });
        }

        protected override Result<MfplTypes> AggregateResult(
            Result<MfplTypes> aggregate,
            Result<MfplTypes> nextResult)
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

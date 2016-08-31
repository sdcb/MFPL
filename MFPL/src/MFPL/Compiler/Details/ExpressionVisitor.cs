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

namespace MFPL.Compiler.Details
{
    public class ExpressionVisitor : MfplBaseVisitor<Result>
    {
        private readonly ILGenerator il;
        private readonly ContextScope<LocalBuilder> scope;

        public ExpressionVisitor(ILGenerator il, ContextScope<LocalBuilder> scope)
        {
            this.il = il;
            this.scope = scope;
        }

        public override Result VisitValueExpression([NotNull] ValueExpressionContext context)
        {
            var node = context.GetChild<ValueContext>(0).GetChild<TerminalNodeImpl>(0).Symbol;
            switch (node.Type)
            {
                case MfplLexer.STRING:
                    return MfplStringUtil.Parse(node.Text).OnSuccess(str =>
                    {
                        il.Emit(OpCodes.Ldstr, str);
                    });
                case MfplLexer.NUMBER:
                    return MfplNumberUtil.Parse(node.Text).OnSuccess(num =>
                    {
                        il.Emit(OpCodes.Ldc_R8, num);
                    });
                case MfplLexer.BOOL:
                    return MfplBoolUtil.Parse(node.Text).OnSuccess(b =>
                    {
                        il.Emit(b ? OpCodes.Ldc_I4_1 : OpCodes.Ldc_I4_0);
                    });
                default:
                    return Result.Fail($"Unkown type for literial '{context.GetText()}'.");
            }
        }
    }
}

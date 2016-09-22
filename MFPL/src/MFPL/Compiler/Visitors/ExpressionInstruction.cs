using MFPL.Compiler.Core;
using MFPL.Functional;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading.Tasks;

namespace MFPL.Compiler.Visitors
{
    public struct ExpressionInstruction
    {
        public ReadOnlyCollection<Instruction> Instructions { get; }

        public MfplTypes ResultType { get; }

        private ExpressionInstruction(IList<Instruction> instructions, MfplTypes resultType)
        {
            Instructions = new ReadOnlyCollection<Instruction>(instructions);
            ResultType = resultType;
        }

        private ExpressionInstruction CopyChangeType(MfplTypes resultType)
        {
            return new ExpressionInstruction(
                Instructions,
                resultType);
        }

        private ExpressionInstruction CopyAddInstruction(params Instruction[] instruction)
        {
            var instructions = new List<Instruction>(Instructions);
            instructions.AddRange(instruction);
            return new ExpressionInstruction(instructions, ResultType);
        }

        private ExpressionInstruction CopyAddInstructionChangeType(
            IList<Instruction> instruction,
            MfplTypes resultType)
        {
            var instructions = new List<Instruction>(Instructions);
            instructions.AddRange(instruction);
            return new ExpressionInstruction(instructions, resultType);
        }

        public Result<ExpressionInstruction> ByUnaryOperation(string op)
        {
            var me = this;
            return MfplTypeUtil.UnaryOperator(op, ResultType).OnSuccess(type =>
            {
                switch (op)
                {
                    case "-":
                        return Result.Ok(me.CopyAddInstruction(
                            Instruction.Create(OpCodes.Neg)));
                    case "!":
                        return Result.Ok(me.CopyAddInstruction(
                            Instruction.Create(OpCodes.Ldc_I4_0),
                            Instruction.Create(OpCodes.Ceq)));
                    default:
                        return Result.Fail<ExpressionInstruction>($"Unknown unary operator: '{op}'.");
                }
            });
        }

        public Result<ExpressionInstruction> ByBinaryOperator(string op, ExpressionInstruction other)
        {
            var me = this;
            return MfplTypeUtil.BinaryOperator(ResultType, other.ResultType, op)
                .OnSuccess(type => me.CopyAddInstructionChangeType(other.Instructions, type))
                .OnSuccess(v =>
                {
                    switch (op)
                    {
                        case "*":
                            return Result.Ok(v.CopyAddInstruction(
                                Instruction.Create(OpCodes.Mul)));
                        case "/":
                            return Result.Ok(v.CopyAddInstruction(
                                Instruction.Create(OpCodes.Div)));
                        case "+":
                            if (v.ResultType == MfplTypes.Number)
                            {
                                return Result.Ok(v.CopyAddInstruction(
                                    Instruction.Create(OpCodes.Add)));
                            }
                            else
                            {
                                var method = typeof(string).GetMethod(
                                    nameof(string.Concat), new[] { typeof(string), typeof(string) });
                                return Result.Ok(v.CopyAddInstruction(
                                    Instruction.Create(OpCodes.Call, method)));
                            }
                        case "-":
                            return Result.Ok(v.CopyAddInstruction(
                                Instruction.Create(OpCodes.Sub)));
                        case ">":
                            return Result.Ok(v.CopyAddInstruction(
                                Instruction.Create(OpCodes.Cgt)));
                        case "<":
                            return Result.Ok(v.CopyAddInstruction(
                                Instruction.Create(OpCodes.Clt)));
                        case ">=":
                            return Result.Ok(v.CopyAddInstruction(
                                Instruction.Create(OpCodes.Clt),
                                Instruction.Create(OpCodes.Ldc_I4_0), 
                                Instruction.Create(OpCodes.Ceq)));
                        case "<=":
                            return Result.Ok(v.CopyAddInstruction(
                                Instruction.Create(OpCodes.Cgt),
                                Instruction.Create(OpCodes.Ldc_I4_0),
                                Instruction.Create(OpCodes.Ceq)));
                        case "&&":
                            return Result.Ok(v.CopyAddInstruction(
                                Instruction.Create(OpCodes.Add)));
                        case "||":
                            return Result.Ok(v.CopyAddInstruction(
                                Instruction.Create(OpCodes.Or)));
                        case "==":
                            if (v.ResultType == MfplTypes.String)
                            {
                                var method = typeof(string).GetMethod(
                                    nameof(string.Compare), new[] { typeof(string), typeof(string) });
                                return Result.Ok(v.CopyAddInstruction(
                                    Instruction.Create(OpCodes.Call, method)));
                            }
                            else
                            {
                                return Result.Ok(v.CopyAddInstruction(
                                    Instruction.Create(OpCodes.Ceq)));
                            }
                        case "!=":
                            if (v.ResultType == MfplTypes.String)
                            {
                                var method = typeof(string).GetMethod(
                                    nameof(string.Compare), new[] { typeof(string), typeof(string) });
                                return Result.Ok(v.CopyAddInstruction(
                                    Instruction.Create(OpCodes.Call, method),
                                    Instruction.Create(OpCodes.Ldc_I4_0),
                                    Instruction.Create(OpCodes.Ceq)));
                            }
                            else
                            {
                                return Result.Ok(v.CopyAddInstruction(
                                    Instruction.Create(OpCodes.Ceq),
                                    Instruction.Create(OpCodes.Ldc_I4_0),
                                    Instruction.Create(OpCodes.Ceq)));
                            }
                        default:
                            return Result.Fail<ExpressionInstruction>($"Unknown binary operator '{op}'.");
                    }
                });
        }

        public static ExpressionInstruction Create(Instruction instruction, MfplTypes resultType)
        {
            return new ExpressionInstruction(
                new List<Instruction> { instruction },
                resultType);
        }
    }
}

using MFPL.Compiler.Core;
using MFPL.Functional;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading.Tasks;

namespace MFPL.Compiler.Core.Instructions
{
    public class ExpressionInstructions : StatementInstruction
    {
        public ReadOnlyCollection<Instruction> Instructions { get; }

        public MfplTypes ResultType { get; }

        public ExpressionInstructions EmitAll(ILGenerator il)
        {
            foreach (var instruction in Instructions)
            {
                instruction.Emit(il);
            }
            return this;
        }

        private ExpressionInstructions(IList<Instruction> instructions, MfplTypes resultType)
        {
            Instructions = new ReadOnlyCollection<Instruction>(instructions);
            ResultType = resultType;
        }

        private ExpressionInstructions CopyChangeType(MfplTypes resultType)
        {
            return new ExpressionInstructions(
                Instructions,
                resultType);
        }

        private ExpressionInstructions CopyAddInstruction(params Instruction[] instruction)
        {
            var instructions = new List<Instruction>(Instructions);
            instructions.AddRange(instruction);
            return new ExpressionInstructions(instructions, ResultType);
        }

        public Result<ExpressionInstructions> ByUnaryOperation(string op)
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
                        return Result.Fail<ExpressionInstructions>($"Unknown unary operator: '{op}'.");
                }
            });
        }

        public Result<ExpressionInstructions> ByBinaryOperator(string op, ExpressionInstructions other)
        {
            var me = this;
            return MfplTypeUtil.BinaryOperator(ResultType, other.ResultType, op)
                .OnSuccess(type => CombineWithType(type, new[] { me, other }))
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
                            if (other.ResultType == MfplTypes.String)
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
                            if (other.ResultType == MfplTypes.String)
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
                            return Result.Fail<ExpressionInstructions>($"Unknown binary operator '{op}'.");
                    }
                });
        }

        public static ExpressionInstructions Create(Instruction instruction, MfplTypes resultType)
        {
            return new ExpressionInstructions(
                new List<Instruction> { instruction },
                resultType);
        }

        public static ExpressionInstructions Create(LocalBuilder localBuilder)
        {
            return new ExpressionInstructions(
                new List<Instruction> { Instruction.Create(OpCodes.Ldloc, localBuilder) },
                MfplTypeUtil.TypeToMfplType(localBuilder.LocalType));
        }

        public static ExpressionInstructions Create(IList<Instruction> instruction, MfplTypes resultType)
        {
            return new ExpressionInstructions(instruction, resultType);
        }

        public static ExpressionInstructions FromValue(string value)
        {
            return new ExpressionInstructions(
                new List<Instruction> { Instruction.Create(OpCodes.Ldstr, value) },
                MfplTypes.String);
        }

        public static ExpressionInstructions FromValue(double value)
        {
            return new ExpressionInstructions(
                new List<Instruction> { Instruction.Create(OpCodes.Ldc_R8, value) },
                MfplTypes.Number);
        }

        public static ExpressionInstructions FromValue(bool value)
        {
            return new ExpressionInstructions(
                new List<Instruction> { Instruction.Create(value ? OpCodes.Ldc_I4_1 : OpCodes.Ldc_I4_0) },
                MfplTypes.Bool);
        }

        public static ExpressionInstructions CombineWithType(
            MfplTypes finalType, 
            params ExpressionInstructions[] eis)
        {
            var instructions = new List<Instruction>();
            foreach (var ei in eis)
            {
                instructions.AddRange(ei.Instructions);
            }
            return new ExpressionInstructions(instructions, finalType);
        }

        public static ExpressionInstructions CombineWithType(
            MfplTypes finalType, 
            IEnumerable<ExpressionInstructions> eis)
        {
            var instructions = new List<Instruction>();
            foreach (var ei in eis)
            {
                instructions.AddRange(ei.Instructions);
            }
            return new ExpressionInstructions(instructions, finalType);
        }
    }
}

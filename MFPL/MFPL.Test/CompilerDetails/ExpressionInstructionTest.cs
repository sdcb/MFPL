using MFPL.Compiler.Core;
using MFPL.Compiler.Core.Instructions;
using MFPL.Compiler.Visitors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Threading.Tasks;
using Xunit;

namespace MFPL.Test.CompilerDetails
{
    public class ExpressionInstructionTest
    {
        [Fact]
        public void SimpleAdd()
        {
            var n1 = ExpressionInstructions.FromValue(3);
            var n2 = ExpressionInstructions.FromValue(4);
            var result = n1.ByBinaryOperator("+", n2);

            Assert.True(result.IsSuccess);

            var value = result.Value;
            Assert.Equal(MfplTypes.Number, value.ResultType);
            var expected = new List<Instruction>
            {
                Instruction.Create(OpCodes.Ldc_R8, 3.0),
                Instruction.Create(OpCodes.Ldc_R8, 4.0),
                Instruction.Create(OpCodes.Add),
            };
            Assert.Equal(expected, value.Instructions);
        }

        [Fact]
        public void StringAdd()
        {
            var n1 = ExpressionInstructions.FromValue("3");
            var n2 = ExpressionInstructions.FromValue("4");
            var result = n1.ByBinaryOperator("+", n2);

            Assert.True(result.IsSuccess);

            var value = result.Value;
            Assert.Equal(MfplTypes.String, value.ResultType);
            Assert.Equal(Instruction.Create(OpCodes.Ldstr, "3"), value.Instructions[0]);
            Assert.Equal(Instruction.Create(OpCodes.Ldstr, "4"), value.Instructions[1]);
            Assert.Equal(InstructionType.MethodInfo, value.Instructions[2].InstructionType);
        }

        [Fact]
        public void NumberGt()
        {
            var n1 = ExpressionInstructions.FromValue(3);
            var n2 = ExpressionInstructions.FromValue(4);
            var result = n1.ByBinaryOperator(">", n2);

            Assert.True(result.IsSuccess);

            var value = result.Value;
            Assert.Equal(MfplTypes.Bool, value.ResultType);
            var expected = new List<Instruction>
            {
                Instruction.Create(OpCodes.Ldc_R8, 3.0),
                Instruction.Create(OpCodes.Ldc_R8, 4.0),
                Instruction.Create(OpCodes.Cgt),
            };
            Assert.Equal(expected, value.Instructions);
        }

        [Fact]
        public void NumberLt()
        {
            var n1 = ExpressionInstructions.FromValue(3);
            var n2 = ExpressionInstructions.FromValue(4);
            var result = n1.ByBinaryOperator("<=", n2);

            Assert.True(result.IsSuccess);

            var value = result.Value;
            Assert.Equal(MfplTypes.Bool, value.ResultType);
            var expected = new List<Instruction>
            {
                Instruction.Create(OpCodes.Ldc_R8, 3.0),
                Instruction.Create(OpCodes.Ldc_R8, 4.0),
                Instruction.Create(OpCodes.Cgt),
                Instruction.Create(OpCodes.Ldc_I4_0),
                Instruction.Create(OpCodes.Ceq),
            };
            Assert.Equal(expected, value.Instructions);
        }

        [Fact]
        public void NumberCompare()
        {
            var n1 = ExpressionInstructions.FromValue(3);
            var n2 = ExpressionInstructions.FromValue(4);
            var result = n1.ByBinaryOperator("!=", n2);

            Assert.True(result.IsSuccess);

            var value = result.Value;
            Assert.Equal(MfplTypes.Bool, value.ResultType);
            var expected = new List<Instruction>
            {
                Instruction.Create(OpCodes.Ldc_R8, 3.0),
                Instruction.Create(OpCodes.Ldc_R8, 4.0),
                Instruction.Create(OpCodes.Ceq),
                Instruction.Create(OpCodes.Ldc_I4_0),
                Instruction.Create(OpCodes.Ceq),
            };
            Assert.Equal(expected, value.Instructions);
        }

        [Fact]
        public void StringCompare()
        {
            var n1 = ExpressionInstructions.FromValue("3");
            var n2 = ExpressionInstructions.FromValue("4");
            var result = n1.ByBinaryOperator("!=", n2);

            Assert.True(result.IsSuccess);

            var value = result.Value;
            Assert.Equal(MfplTypes.Bool, value.ResultType);
            Assert.Equal(Instruction.Create(OpCodes.Ldstr, "3"), value.Instructions[0]);
            Assert.Equal(Instruction.Create(OpCodes.Ldstr, "4"), value.Instructions[1]);
            Assert.Equal(InstructionType.MethodInfo, value.Instructions[2].InstructionType);
            Assert.Equal(Instruction.Create(OpCodes.Ldc_I4_0), value.Instructions[3]);
            Assert.Equal(Instruction.Create(OpCodes.Ceq), value.Instructions[4]);
        }
    }
}

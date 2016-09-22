using MFPL.Compiler;
using MFPL.Compiler.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace MFPL.Test.CompilerDetails
{
    public class MfplTypeUtilTest
    {
        [Theory]
        // +-*/
        [InlineData(MfplTypes.String, MfplTypes.String, "+", MfplTypes.String)]
        [InlineData(MfplTypes.Number, MfplTypes.Number, "+", MfplTypes.Number)]
        [InlineData(MfplTypes.Number, MfplTypes.Number, "-", MfplTypes.Number)]
        [InlineData(MfplTypes.Number, MfplTypes.Number, "*", MfplTypes.Number)]
        [InlineData(MfplTypes.Number, MfplTypes.Number, "/", MfplTypes.Number)]

        // == != compare
        [InlineData(MfplTypes.Number, MfplTypes.Number, "==", MfplTypes.Bool)]
        [InlineData(MfplTypes.String, MfplTypes.String, "==", MfplTypes.Bool)]
        [InlineData(MfplTypes.Bool, MfplTypes.Bool, "==", MfplTypes.Bool)]
        [InlineData(MfplTypes.String, MfplTypes.String, "!=", MfplTypes.Bool)]

        // > < >= <= compare
        [InlineData(MfplTypes.Number, MfplTypes.Number, ">=", MfplTypes.Bool)]
        [InlineData(MfplTypes.Number, MfplTypes.Number, "<=", MfplTypes.Bool)]
        [InlineData(MfplTypes.Number, MfplTypes.Number, ">", MfplTypes.Bool)]
        [InlineData(MfplTypes.Number, MfplTypes.Number, "<", MfplTypes.Bool)]

        // logic
        [InlineData(MfplTypes.Bool, MfplTypes.Bool, "&&", MfplTypes.Bool)]
        [InlineData(MfplTypes.Bool, MfplTypes.Bool, "||", MfplTypes.Bool)]
        public void BinaryOperation(MfplTypes type1, MfplTypes type2, string op, MfplTypes expected)
        {
            var result = MfplTypeUtil.BinaryOperator(type1, type2, op);
            Assert.True(result.IsSuccess);
            Assert.Equal(expected, result.Value);
        }

        [Theory]
        [InlineData(MfplTypes.String, MfplTypes.Number, "+")]
        [InlineData(MfplTypes.String, MfplTypes.String, "-")]
        [InlineData(MfplTypes.String, MfplTypes.String, "&&")]
        [InlineData(MfplTypes.Number, MfplTypes.Number, "||")]
        [InlineData(MfplTypes.Number, MfplTypes.Number, "unknown operator")]
        public void FailBinaryOperator(MfplTypes type1, MfplTypes type2, string op)
        {
            var result = MfplTypeUtil.BinaryOperator(type1, type2, op);
            Assert.True(result.IsFailure);
        }

        [Theory]
        [InlineData("+", MfplTypes.Number, MfplTypes.Number)]
        [InlineData("-", MfplTypes.Number, MfplTypes.Number)]
        [InlineData("!", MfplTypes.Bool, MfplTypes.Bool)]
        public void UnaryOperator(string op, MfplTypes type, MfplTypes expected)
        {
            var result = MfplTypeUtil.UnaryOperator(op, type);
            Assert.True(result.IsSuccess);
            Assert.Equal(expected, result.Value);
        }

        [Theory]
        [InlineData("+", MfplTypes.String)]
        [InlineData("!", MfplTypes.Number)]
        [InlineData("unknown operator", MfplTypes.Number)]
        public void FailUnaryOperator(string op, MfplTypes type)
        {
            var result = MfplTypeUtil.UnaryOperator(op, type);
            Assert.True(result.IsFailure);
        }
    }
}

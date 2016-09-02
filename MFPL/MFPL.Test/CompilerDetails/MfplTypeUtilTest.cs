using MFPL.Compiler.Details;
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
        [InlineData(MfplTypes.String, MfplTypes.String, "+", MfplTypes.String)]
        [InlineData(MfplTypes.Number, MfplTypes.Number, "+", MfplTypes.Number)]
        [InlineData(MfplTypes.Number, MfplTypes.Number, "-", MfplTypes.Number)]
        [InlineData(MfplTypes.Number, MfplTypes.Number, "*", MfplTypes.Number)]
        [InlineData(MfplTypes.Number, MfplTypes.Number, "/", MfplTypes.Number)]
        [InlineData(MfplTypes.Number, MfplTypes.Number, "==", MfplTypes.Bool)]
        [InlineData(MfplTypes.String, MfplTypes.String, "==", MfplTypes.Bool)]
        [InlineData(MfplTypes.Bool, MfplTypes.Bool, "==", MfplTypes.Bool)]
        [InlineData(MfplTypes.Bool, MfplTypes.Bool, "&&", MfplTypes.Bool)]
        [InlineData(MfplTypes.Bool, MfplTypes.Bool, "||", MfplTypes.Bool)]
        public void SuccessBinaryOperation(MfplTypes type1, MfplTypes type2, string op, MfplTypes expected)
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
        public void FailBinaryOperator(MfplTypes type1, MfplTypes type2, string op)
        {
            var result = MfplTypeUtil.BinaryOperator(type1, type2, op);
            Assert.True(result.IsFailure);
        }
    }
}

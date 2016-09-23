﻿using MFPL.Compiler;
using MFPL.Compiler.Core;
using MFPL.Compiler.MfplLibs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace MFPL.Test.CompilerDetails
{
    public class KnownFunctionTest
    {
        [Fact]
        public void HelloWorld()
        {
            var func = KnownFunction.GetFunction("printHelloWorld", new MfplTypes[] { });
            Assert.True(func.IsSuccess);
            Assert.Equal(MfplTypes.Void, func.Value.ResultType);
        }

        [Fact]
        public void WrontSyntaxCannotFind()
        {
            var func = KnownFunction.GetFunction("printHelloWorld2", new MfplTypes[] { });
            Assert.True(func.IsFailure);
        }

        [Fact]
        public void WrontParameterCannotFind()
        {
            var func = KnownFunction.GetFunction("printHelloWorld", new MfplTypes[] { MfplTypes.Number });
            Assert.True(func.IsFailure);
        }
    }
}

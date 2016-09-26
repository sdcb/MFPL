using MFPL.Compiler.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace MFPL.Test.CompilerDetails
{
    public class ContextScopeTest
    {
        [Fact]
        public void SetGetOk()
        {
            var scope = ContextScope.CreateEmpty<string>();
            var kv = new { Key = "key", Value = "value" };

            var setResult = scope.Declare(kv.Key, kv.Value);
            Assert.True(setResult.IsSuccess);
            Assert.Equal(kv.Value, setResult.Value);

            var getResult = scope.Get(kv.Key);
            Assert.True(getResult.IsSuccess);
            Assert.Equal(kv.Value, getResult.Value);
        }

        [Fact]
        public void CannotGetNotExistedKey()
        {
            var scope = ContextScope.CreateEmpty<string>();
            var getResult = scope.Get("not_existed_key");
            Assert.True(getResult.IsFailure);
        }

        [Fact]
        public void CanGetNestedScope()
        {
            var outScope = ContextScope.CreateEmpty<string>();
            var inscope = ContextScope.CreateInScope(outScope);
            var kv = new { Key = "key", Value = "value" };
            outScope.Declare(kv.Key, kv.Value);

            var getResult = inscope.Get(kv.Key);
            Assert.True(getResult.IsSuccess);
            Assert.Equal(kv.Value, getResult.Value);
        }

        [Fact]
        public void CannotDeclareSameInSameScope()
        {
            var scope = ContextScope.CreateEmpty<string>();
            var kv = new { Key = "key", Value = "value" };

            var result1 = scope.Declare(kv.Key, kv.Value);
            var result2 = scope.Declare(kv.Key, kv.Value);

            Assert.True(result1.IsSuccess);
            Assert.True(result2.IsFailure);
        }

        [Fact]
        public void CannotDeclareSameInNestedScope()
        {
            var outScope = ContextScope.CreateEmpty<string>();
            var inScope = ContextScope.CreateInScope(outScope);
            var kv = new { Key = "key", Value = "value" };

            var outResult = outScope.Declare(kv.Key, kv.Value);
            var inResult = inScope.Declare(kv.Key, kv.Value);

            Assert.True(outResult.IsSuccess);
            Assert.True(inResult.IsFailure);
        }
    }
}
